using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    public void isSelected(Item item)
    {
        if(item==null)
        {
            return;
        }
        else
        {
            InfoDisplay.instance.selected = true;
            InfoDisplay.instance.item = item;
        }
    }
}
