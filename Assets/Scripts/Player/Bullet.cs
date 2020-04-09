using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    private void Start()
    {
    }
    void Update()
    {
        transform.Translate(Camera.main.transform.forward*speed*Time.deltaTime);
    }
}
