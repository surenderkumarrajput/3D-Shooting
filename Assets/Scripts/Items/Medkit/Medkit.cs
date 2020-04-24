using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Medkit", menuName = "Items/Medkit")]

public class Medkit : Item
{
    [SerializeField]
    private float healthIncrease;
    public override void Use(Item _Item)
    {
        var PlayerWeapons = GameObject.FindGameObjectWithTag("Player");
        for (int i = 0; i < PlayerWeapons.GetComponent<Inventory>().Container.Count; i++)
        {
            if(PlayerWeapons.GetComponent<Inventory>().Container[i].item.name==_Item.name)
            {
                PlayerWeapons.GetComponent<HealthSystems>().IncreaseHealth(healthIncrease);
            }
        }
    }
}
