using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemGizmos : MonoBehaviour
{
    Item item;
    public LayerMask Layer;
    private TextMeshProUGUI Text;
    private float ElapsedTime = 0;
    private float fixedtime = 3f;
    private void Start()
    {
        item = GetComponent<ItemScript>().item;
        Text = GameObject.Find("Press E to pick").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 3, Layer);
        foreach (var hit in collider)
        {
            Text.text = "Press E to pick " + item.Itemname;
            if (Input.GetKeyDown(KeyCode.E))
            {
                Text.text = "";
                hit.GetComponent<PlayerController>().AddtoInventory(this.item);
                Destroy(gameObject);
            }
        }
        if (Text.text != "" && ElapsedTime > fixedtime)
        {
            Text.text = "";
            ElapsedTime = 0f;
        }
        else
        {
            ElapsedTime += Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3);
    }
}
