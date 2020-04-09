using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour
{
    public void isSelected(Item item)
    {
        InfoDisplay.instance.selected = true;
        InfoDisplay.instance.item = item;
    }
}
