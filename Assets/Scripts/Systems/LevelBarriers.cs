using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBarriers : MonoBehaviour
{
    public List<GameObject> EnemiesAlive = new List<GameObject>();
    void Update()
    {
        if(EnemiesAlive.Count==0)
        {
            Destroy(gameObject);
        }
    }
}
