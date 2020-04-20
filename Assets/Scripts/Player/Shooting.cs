using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(Input.GetMouseButton(0) && Time.time >= TimebetweenFire && CurrentAmmo>0&&!isReloading)
        {
            StartCoroutine(ShootingFunction(weapons.Range));
            TimebetweenFire = Time.time + 1 / weapons.FireRate;
        }
        if(((Input.GetKeyDown(KeyCode.R)&&((CurrentAmmo < weapons.MaxBullets) || (CurrentAmmo <= 0)))&&weapons.TotalBullets>0))
        {
            StartCoroutine(Reloading());
            return;
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
            }
        }
        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (inventory.Container[i].item == ammo)
            {
                weapons.TotalBullets = inventory.Container[i].amount;
            }
        }
        CurrentAmmo--;
        Instantiate(MuzzleFlash, transform.position, Quaternion.identity);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * weapons.Range;
        Gizmos.DrawRay(transform.position, direction);
    }
}
