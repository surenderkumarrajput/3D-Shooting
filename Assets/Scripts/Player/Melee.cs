using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Melee : MonoBehaviour
{
    public LayerMask Layers;

    public Weapons weapon;

    float TimebetweenFiring;

    Animator animator;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
    }
    void Update()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, weapon.Range, Layers);
        if(Input.GetMouseButton(0)&&Time.time>=TimebetweenFiring&&!MouseoveronGUI())
        {
            animator.SetTrigger("Attack");
            foreach (var Temp in collider)
            {
                Temp.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapon.Damage);
            }
            TimebetweenFiring = Time.time + 1 / weapon.FireRate;
        }
    }
    public bool MouseoveronGUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastresult = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastresult);
        for (int i = 0; i < raycastresult.Count; i++)
        {
            if (raycastresult[i].gameObject.GetComponent<PassThroughClick>() != null)
            {
                raycastresult.RemoveAt(i);
                i--;
            }
        }
        return raycastresult.Count > 0;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,weapon.Range);
    }
}
