using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    public GameObject Inventory;
    void Start()
    {
        Inventory.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            Inventory.SetActive(true);
        }
        else
        {
            Inventory.SetActive(false);
        }
    }
}
