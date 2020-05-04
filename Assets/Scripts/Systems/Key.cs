﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public HealthSystems healthSystems;

    public GameObject Key_Object;
    
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
        Instantiate(Key_Object, transform.position, Quaternion.identity);
    }
}
