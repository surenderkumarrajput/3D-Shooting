using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;
using UnityEngine.UI;

public enum States
{
    IDLE,Attacking,Running,Patroling
}
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navmeshAgent;

    public float Range,SpottingRange;
    public float Damage;
    public float TimeBetweenFiring;
    public float FireRate,SpawnPointsRate;
    public float Waittime;
    public float MinX, MaxX;
    public float MinZ, MaxZ;

    public GameObject BloodImpact;
    public GameObject MuzzleFlash;
    public GameObject Death_Effect;

    private HealthSystems HealthSystems;

    private bool isAlive = true;

    public Transform trigger;
    public Transform Centre;
    public Transform Hips_Position;
    public Transform MovePoints;

    public Image HealthImage;

    public States states;

    private Animator anim;

    private Vector3 PatrolPoints;

    public PlayerController playerController;

    void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        navmeshAgent.stoppingDistance = Range;
        HealthSystems = GetComponent<HealthSystems>();
        anim = GetComponent<Animator>();
        states = States.IDLE;
        PlayerController.Playerdeath += StopEnemyMovements;
        PatrolPoints = new Vector3(MovePoints.position.x+Random.Range(MinX, MaxX), 0, MovePoints.position.z + Random.Range(MinZ, MaxZ));
    }

    void Update()
    {
        HealthImage.fillAmount = HealthSystems.CurrentHealth / HealthSystems.MaxHealth;
        if(HealthSystems.CurrentHealth<=0)
        {
            isAlive = false;
            anim.SetTrigger("Death");
            Destroy(gameObject,3f);
        }
        if(isAlive)
        {
            //Shooting
            if (Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position) <= navmeshAgent.stoppingDistance)
            {
                states = States.Attacking;
            }
            #region AI
            switch (states)
            {
                case States.IDLE:
                    {
                        navmeshAgent.ResetPath();
                        break;
                    }
                case States.Running:
                    {
                        navmeshAgent.SetDestination(playerController.GetComponent<Transform>().position);
                        break;
                    }
                case States.Attacking:
                    {
                        var Vector = (playerController.GetComponent<Transform>().position - transform.position).normalized;
                        Quaternion LookRotation = Quaternion.LookRotation(new Vector3(Vector.x, 0, Vector.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 10f);
                        if(Time.time >= TimeBetweenFiring)
                        {
                            anim.SetTrigger("NormalShoot");
                            TimeBetweenFiring = Time.time + 1 / FireRate;
                        }
                        break;
                    }
                case States.Patroling:
                {
                        if(Time.time>=Waittime)
                        {
                            Waittime = Time.time+1/SpawnPointsRate;
                            EnemyPatrolPoints();
                        }
                        navmeshAgent.SetDestination(PatrolPoints);
                        break;
                }
            }
            anim.SetFloat("Speed", navmeshAgent.velocity.magnitude, .1f, Time.deltaTime);
            #endregion

            #region Follow
            Collider[] collider = Physics.OverlapSphere(trigger.transform.position, SpottingRange);
            foreach (var Temp in collider)
            {
                if (Temp.gameObject.CompareTag("Player"))
                {
                    states = States.Running;
                }
            }
            if (Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position) > SpottingRange)
            {
                StartCoroutine(IdleToPatrol());
            }
            #endregion
        }
        if(!isAlive)
        {
            navmeshAgent.ResetPath();
            anim.SetFloat("Speed", 0);
            anim.ResetTrigger("NormalShoot");
        }
    }
    public void Shoot()
    {
        Ray ray = new Ray(Centre.transform.position, Centre.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Range) )
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.GetComponent<HealthSystems>().DecreaseHealth(Damage);
                CameraShaker.Instance.ShakeOnce(20, 20, .1f, .2f);
                Instantiate(BloodImpact, hit.collider.GetComponent<Transform>().position, Quaternion.identity);
            }
        }
    }
    public void DeathEffect()
    {
        Instantiate(Death_Effect, Hips_Position.position, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * Range;
        Gizmos.DrawRay(Centre.transform.position, direction);
    }
    public void MuzzleFlashEffect()
    {
        Instantiate(MuzzleFlash, trigger.position, transform.rotation);
    }
    public void StopEnemyMovements()
    {
        isAlive = false;
    }
    void EnemyPatrolPoints()
    {
        PatrolPoints = new Vector3(MovePoints.position.x + Random.Range(MinX, MaxX), 0, MovePoints.position.z + Random.Range(MinZ, MaxZ));
    }
    IEnumerator IdleToPatrol()
    {
        states = States.IDLE;
        yield return new WaitForSeconds(5f);
        states = States.Patroling;
    }
}
