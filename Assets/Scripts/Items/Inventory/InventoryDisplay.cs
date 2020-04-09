using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    Inventory inventory;
    Dictionary<InventorySlot, GameObject> dic = new Dictionary<InventorySlot, GameObject>();
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    void Update()
    {
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
           if(dic.ContainsKey(inventory.Container[i]))
            {
                dic[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString();
                if(inventory.Container[i].amount==0)
                {
                    Destroy(dic[inventory.Container[i]]);
                    inventory.Container.Remove(inventory.Container[i]);
                }
            }
            else
            {
                var go =Instantiate(inventory.Container[i].item.InventoryImages, transform, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString();
                dic.Add(inventory.Container[i],go);
            }
        }
    }
}
