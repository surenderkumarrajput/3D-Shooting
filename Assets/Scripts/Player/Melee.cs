using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Melee : MonoBehaviour
{
    public LayerMask Layers;

    public Weapons weapon;

    public Transform Knife_pos;

    private float TimebetweenFiring;

    Animator animator;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }
    void Update()
    {
        if(Input.GetMouseButton(0)&&Time.time>=TimebetweenFiring&&!MouseoveronGUI())
        {
            animator.SetTrigger("Attack");
            TimebetweenFiring = Time.time + 1 / weapon.FireRate;
        }
    }
    public bool MouseoveronGUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastresult = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastresult);
        for (int i = 0; i < raycastresult.Count; i++)
        {
            if (raycastresult[i].gameObject.GetComponent<PassThroughClick>() != null)
            {
                raycastresult.RemoveAt(i);
                i--;
            }
        }
        return raycastresult.Count > 0;
    }
    public void MeleeAttack()
    {
        Collider[] collider = Physics.OverlapSphere(Knife_pos.position, weapon.Range, Layers);
        foreach (var Temp in collider)
        {
            if (Temp.gameObject.GetComponent<HealthSystems>() != null)
            {
                if (Temp.gameObject.GetComponent<StunnSystem>() != null)
                {
                    if (Temp.gameObject.GetComponent<StunnSystem>().CurrentStunn != 0)
                    {
                        float Remaining_Stunn = Temp.gameObject.GetComponent<StunnSystem>().DecreaseStunn(weapon.Damage);
                        if (Remaining_Stunn < 0)
                        {
                            Remaining_Stunn = 0;
                        }
                    }
                    else
                    {
                        Temp.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapon.Damage);
                    }
                }
                else
                {
                    Temp.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapon.Damage);
                }
            }
            if(Temp.gameObject.GetComponent<EnemyController>()!=null)
            {
                FindObjectOfType<AudioManager>().Play(Temp.gameObject.GetComponent<EnemyController>().ImpactSound);
            }
            if (Temp.gameObject.GetComponent<Boss>() != null)
            {
                FindObjectOfType<AudioManager>().Play("BloodImpact");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Knife_pos.position,weapon.Range);
    }
}
