using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EZCameraShake;
using System.Runtime.InteropServices.WindowsRuntime;

public class Shooting : MonoBehaviour
{
    public Weapons weapons;
    public Ammo ammo;

    Animator animator;

    Inventory inventory;

    float TimebetweenFire;
    public float CurrentAmmo=0;
    public float ReloadTime;


   public GameObject MuzzleFlash;

    bool isReloading;

    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
        CurrentAmmo = weapons.MaxBullets;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= TimebetweenFire && CurrentAmmo>0&&!isReloading&&!MouseoveronGUI())
        {
            StartCoroutine(ShootingFunction(weapons.Range));
            TimebetweenFire = Time.time + 1 / weapons.FireRate;
        }
        if(Input.GetKeyDown(KeyCode.R)&&(CurrentAmmo<weapons.MaxBullets&&weapons.TotalBullets>0))
        {
            StartCoroutine(Reloading());
            return;
        }
        foreach (var item in inventory.Container)
        {
            if (item.item == ammo)
            {
                weapons.TotalBullets = item.amount;
            }
        }
        CurrentAmmo = Mathf.Clamp(CurrentAmmo, 0, weapons.MaxBullets);
    }
    IEnumerator ShootingFunction(float Range)
    {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,Range))
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<HealthSystems>().DecreaseHealth(weapons.Damage);
                CameraShaker.Instance.ShakeOnce(20, 20, .1f, .2f);
            }
        }
        Instantiate(MuzzleFlash, transform.position, transform.rotation);
        CurrentAmmo--;
    }
    IEnumerator Reloading()
    {
        isReloading = true;
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(ReloadTime);
        animator.ResetTrigger("Reload");
        weapons.TotalBullets--;
        CurrentAmmo = weapons.MaxBullets;
        isReloading = false;
        weapons.TotalBullets=Mathf.Clamp(weapons.TotalBullets,0,weapons.TotalBullets);
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (inventory.Container[i].item == ammo)
            {
                inventory.Container[i].amount = (int)weapons.TotalBullets;
            }
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
            if(raycastresult[i].gameObject.GetComponent<PassThroughClick>()!=null)
            {
                raycastresult.RemoveAt(i);
                i--;
            }
        }
        return raycastresult.Count>0;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * weapons.Range;
        Gizmos.DrawRay(transform.position, direction);
    }
}
