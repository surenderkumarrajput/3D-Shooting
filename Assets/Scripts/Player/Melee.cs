using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Melee : MonoBehaviour
{
    public LayerMask Layers;

    public Weapons weapon;

    float TimebetweenFiring;

    Animator animator;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Collider[] collider = Physics.OverlapSphere(transform.position, weapon.Range, Layers);
        if(Input.GetMouseButton(0)&&Time.time>=TimebetweenFiring)
        {
            animator.SetTrigger("Attack");
            foreach (var Temp in collider)
            {
                Temp.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapon.Damage);
            }
            TimebetweenFiring = Time.time + 1 / weapon.FireRate;
        }
    }
   
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,weapon.Range);
    }
}
