using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Key : MonoBehaviour
{
    private HealthSystems healthSystems;
    public WayPoint Waypoint_Ref;
    public Vector3 Offset;

    public GameObject Key_Object;
    public GameObject WayPoint_Object;

    private bool isEnemyDead = false;
    private void Start()
    {
        healthSystems = GetComponent<HealthSystems>();
    }
    private void Update()
    {
        if(healthSystems.CurrentHealth<=0 && !isEnemyDead)
        {
            isEnemyDead = true;
            Key_Spawn();
        }
    }
    public void Key_Spawn()
    {
        GameObject go = Instantiate(Key_Object, GetComponent<EnemyController>().Centre.position+Offset, Quaternion.identity);
        Waypoint_Ref.Target.Add(go.transform);
        Waypoint_Ref.i++;
        WayPoint_Object.SetActive(true);
        Objectives.instance.i++;
    }
}
