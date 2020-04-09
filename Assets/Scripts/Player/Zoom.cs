using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public GameObject crosshair;
    float FOV;
    public float rate;
    void Start()
    {
        crosshair.SetActive(false);
        FOV=GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            FOV -= rate;
            crosshair.SetActive(true);
        }
        else
        {
            FOV += rate;
            crosshair.SetActive(false);
        }
        FOV = Mathf.Clamp(FOV, 45, 60);
        GetComponent<Camera>().fieldOfView=FOV;
    }
}
