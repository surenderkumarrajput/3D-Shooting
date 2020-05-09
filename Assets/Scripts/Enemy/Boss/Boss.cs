using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;
using UnityEngine.UI;

public enum Boss_States
{
    IDLE, Attacking, Running
}
public class Boss : MonoBehaviour
{
    private NavMeshAgent NavmeshAgent;

    private Boss_States States;

    public float Range;
    public float AttackingDistance;
    private float ElapsedTime;
    private float FixedTime = 1.3f;

    public PlayerController playerController;
    private HealthSystems HealthSystems;
    private EnergySystem EnergySystem;
    private StunnSystem StunnSystem;

    public Image Health_Img;
    public Image Energy_Img;
    public Image Stunn_Img;

    public GameObject[] Object_Spawn;

    public Transform Right_Hand_Axe;
    public Transform Hips_Transform;

    public LayerMask Player_Layer;

    private Animator anim;

    private bool isSaid1=false;
    private bool isSaid2 = false;

    [HideInInspector]
    public bool isDead = false;
    private bool isStunn = false;
    void Start()
    {
        NavmeshAgent = GetComponent<NavMeshAgent>();
        States = Boss_States.IDLE;
        anim = GetComponent<Animator>();
        HealthSystems = GetComponent<HealthSystems>();
        PlayerController.Playerdeath += StopEnemyMovements;
        NavmeshAgent.stoppingDistance = AttackingDistance;
        NavmeshAgent.speed = 9;
        NavmeshAgent.acceleration = NavmeshAgent.speed;
        EnergySystem = GetComponent<EnergySystem>();
        StunnSystem = GetComponent<StunnSystem>();
    }

    void Update()
    {
        Health_Img.fillAmount = HealthSystems.CurrentHealth / HealthSystems.MaxHealth;
        Energy_Img.fillAmount = EnergySystem.CurrentEnergy / EnergySystem.MaxEnergy;
        Stunn_Img.fillAmount = StunnSystem.CurrentStunn / StunnSystem.maxStunn;
       
        if (HealthSystems.CurrentHealth <= 0)
        {
            isDead = true;
            anim.SetTrigger("Die");
            Destroy(gameObject, 2.6f);
        }
        if (StunnSystem.CurrentStunn <= 0)
        {
            anim.SetBool("isStunn", true);
            if(!isSaid2)
            {
                isSaid2 = true;
                var rand = Random.Range(0,Object_Spawn.Length);
                Instantiate(Object_Spawn[rand],transform.localPosition, Quaternion.identity);
                FindObjectOfType<AudioManager>().Play("Stunn");
            }
            StunnSystem.StunnIncrease();
            isStunn = true;
        }
        else
        {
            isSaid2 = false;
            anim.SetBool("isStunn", false);
            isStunn = false;
        }
        if (!isDead&&!isStunn)
        {
            #region AI
            switch (States)
            {
                case Boss_States.IDLE:
                    {
                       
                        NavmeshAgent.isStopped=true;
                        break;
                    }
                case Boss_States.Attacking:
                    {
                        NavmeshAgent.isStopped = true;
                        var Vector = (playerController.GetComponent<Transform>().position - transform.position).normalized;
                        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(Vector.x, 0, Vector.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 10f);
                        if (ElapsedTime > FixedTime)
                        {
                            if (EnergySystem.CurrentEnergy == EnergySystem.MaxEnergy)
                            {
                                anim.SetTrigger("SpecialAttack");
                            }
                            else if(EnergySystem.CurrentEnergy < EnergySystem.MaxEnergy)
                            {
                               
                                int rand = Random.Range(0, 2);
                                if (rand == 1)
                                {
                                    anim.SetTrigger("Attack1");
                                }
                                if (rand == 0)
                                {
                                    anim.SetTrigger("Attack2");
                                }
                            }
                            ElapsedTime = 0f;
                        }
                        else
                        {
                            ElapsedTime += Time.deltaTime;
                        }
                        if (Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position) > AttackingDistance)
                        {
                            States = Boss_States.Running;
                        }
                        break;

                    }
                case Boss_States.Running:
                    {
                        NavmeshAgent.isStopped = false;
                        if (!isSaid1)
                        {
                            isSaid1 = true;
                            FindObjectOfType<DialogAudioManager>().Play("u think u can kill me ");
                        }
                        NavmeshAgent.SetDestination(playerController.GetComponent<Transform>().position);
                        break;
                    }
            }
            if (EnergySystem.CurrentEnergy < EnergySystem.MaxEnergy)
            {
                EnergySystem.EnergyIncrease();
            }
            Collider[] Collider = Physics.OverlapSphere(transform.position, Range, Player_Layer);
            foreach (var hit in Collider)
            {
                States = Boss_States.Running;
            }
            if (Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position) <= AttackingDistance)
            {
                States = Boss_States.Attacking;
            }
           
            if (Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position) > Range)
            {
                StartCoroutine(IdletoChase());
            }
            #endregion
            anim.SetFloat("Speed", NavmeshAgent.velocity.sqrMagnitude,0.1f,Time.deltaTime);
        }
        else
        {
            NavmeshAgent.ResetPath();
            anim.SetFloat("Speed", 0f);
            anim.ResetTrigger("Attack1");
            anim.ResetTrigger("Attack2");
            anim.ResetTrigger("SpecialAttack");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.DrawWireSphere(Right_Hand_Axe.position, 1);
    }
    IEnumerator IdletoChase()
    {
        States = Boss_States.IDLE;
        yield return new WaitForSeconds(1f);
        States = Boss_States.Running;
    }
    public void Attack_Effect_Function(GameObject Effect_Object)
    {
        Instantiate(Effect_Object, Right_Hand_Axe.position, Quaternion.identity);
    }
    public void Special_Attack_Energy()
    {
        FindObjectOfType<DialogAudioManager>().Play("RestinPeace");
        EnergySystem.CurrentEnergy = 0f;
    }
    public void Attack(float Damage)
    {
        Collider[] Collider = Physics.OverlapSphere(Right_Hand_Axe.position, 2,Player_Layer);
        foreach (var hit in Collider)
        {
            FindObjectOfType<AudioManager>().Play("BloodImpact");
            hit.gameObject.GetComponent<HealthSystems>().DecreaseHealth(Damage);
            CameraShaker.Instance.ShakeOnce(20, 20, .1f, .2f);
        }
    }

    public void Death_Effect_Function(GameObject Death_Effect)
    {
        FindObjectOfType<AudioManager>().Play("Explosion");
        Instantiate(Death_Effect, Hips_Transform.position,Quaternion.identity);
    }
    private void StopEnemyMovements()
    {
        isDead = true;
    }
}
    
