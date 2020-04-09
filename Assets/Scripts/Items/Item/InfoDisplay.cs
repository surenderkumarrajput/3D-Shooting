using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    public static InfoDisplay instance;
    public Item item;
    public bool selected = false;
    void Start()
    {
        item = null;
        instance = this;
    }
    public void Use()
    {
        if(selected)
        {
            item.Use();
        }
        else
        {
            return;
        }
    }
}
