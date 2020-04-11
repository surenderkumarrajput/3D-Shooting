using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent navmeshAgent;

    public float Range;

    public LayerMask layers;

    public PlayerController playerController;

    void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        navmeshAgent.stoppingDistance = 4f;
    }

    void Update()
    {
        Follow();
        EnemyShoot();
        if(Vector3.Distance(transform.position, playerController.GetComponent<Transform>().position)>Range)
        {
            navmeshAgent.ResetPath();
        }
    }
    void Follow()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, Range, layers);
        foreach (var Temp in collider)
        {
            navmeshAgent.SetDestination(playerController.GetComponent<Transform>().position);
        }
    }
    void EnemyShoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Range, layers))
        {
            Debug.Log("Shooting Player");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
