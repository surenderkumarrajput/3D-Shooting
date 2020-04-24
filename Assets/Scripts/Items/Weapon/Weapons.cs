using UnityEngine;
[CreateAssetMenu(fileName ="Weapon",menuName ="Items/Weapon")]
public class Weapons : Item
{
    public float Damage;
    public float MaxBullets;
    public float Range;
    public float FireRate;
    public float TotalBullets = 0;
    public GameObject Model;
    GameObject go;
    public override void Use(Item _Item)
    {
        var PlayerWeapons = GameObject.FindGameObjectWithTag("Player");

        if (PlayerWeapons.GetComponent<PlayerController>().WeaponList.Contains(go))
        {
            return;
        }
        else
        {
            go = Instantiate(Model, Camera.main.transform);
            go.SetActive(false);
            PlayerWeapons.GetComponent<PlayerController>().WeaponList.Add(go);
        }
      
    }
}
