using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    public static InfoDisplay instance;
    PlayerController PlayerController;

    Inventory inventory;

    public Item item;
    public bool selected = false;
    void Start()
    {
        item = null;
        instance = this;
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        inventory= GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    public void Use()
    {
        if(selected==true)
        {
            item.Use(item);
            RemoveItem(item);
            selected = false;
        }
        else
        {
            return;
        }
    }
    public void Drop()
    {
        if (selected == true)
        {
            PlayerController.Spawn(item.SpawnableObjects);
            RemoveItem(item);
            selected = false;
        }
        else
        {
            return;
        }
    }
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (inventory.Container[i].item.Itemname == _item.Itemname)
            {
                if (inventory.Container[i].amount == 0)
                {
                    inventory.Container.Remove(inventory.Container[i]);
                }
                else if (inventory.Container[i].amount > 0)
                {
                    inventory.Container[i].amount--;
                }
            }
        }
    }
    }
