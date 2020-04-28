using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
        {
            return;
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
