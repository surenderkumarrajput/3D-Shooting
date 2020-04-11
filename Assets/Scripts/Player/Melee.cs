using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public LayerMask Layers;

    public Weapons weapon;

    Animator animator;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }
    void Update()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, weapon.Range, Layers);
        if(Input.GetMouseButton(0))
        {
            animator.SetTrigger("Attack");
            foreach (var Temp in collider)
            {
                Temp.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapon.Damage);
            }
        }
    }
   
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,weapon.Range);
    }
}
