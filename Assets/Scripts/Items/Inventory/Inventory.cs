using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnitemChanged();
    public OnitemChanged onitemChangedCallback;
    public List<InventorySlot> Container = new List<InventorySlot>();
    public void Add(Item _item,int _amount)
    {
        bool hasitem = false;
        for (int i = 0; i < Container.Count; i++)
        {
            if(Container[i].item==_item)
            {
                Container[i].AddAmount(_amount);
                hasitem = true;
                break;
            }
        }
        if(!hasitem)
        {
            Container.Add(new InventorySlot(_item,_amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int amount;
    public InventorySlot(Item _item,int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int _amount)
    {
        amount += _amount;
    }
    public void DecAmount(int _amount)
    {
        amount -= _amount;
    }
}
