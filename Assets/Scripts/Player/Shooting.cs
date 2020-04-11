using Mirror.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Weapons weapons;

    Animator animator;

    public LayerMask layers;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootingFunction(weapons.Range));
        }
    }
     IEnumerator ShootingFunction(float Range)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,Range,layers))
        {
            hit.collider.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapons.Damage);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward)*100 ;
        Gizmos.DrawRay(transform.position, direction);
    }
}
