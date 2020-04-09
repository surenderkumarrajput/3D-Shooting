using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public string Itemname;
    public GameObject InventoryImages;
    public virtual void Use()
    {
        Debug.Log("Use");
    }
}
