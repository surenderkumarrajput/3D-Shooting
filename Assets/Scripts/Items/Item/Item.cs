using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    public string Itemname;
    public int Amount;
    public GameObject InventoryImages;
    public GameObject SpawnableObjects;
    public virtual void Use(Item _Item)
    {
        Debug.Log("Use");
    }
}
