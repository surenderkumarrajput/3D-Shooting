using UnityEngine;
[CreateAssetMenu(fileName ="Weapon",menuName ="Items/Weapon")]
public class Weapons : Item
{
    public float Damage;
    public float MaxBullets;
    public float Range;
    public float FireRate;
    public float TotalBullets = 0;

    public string ReloadSound;
    public string FireSound;
    public string ReadySound;

    public GameObject Model;
    private GameObject go;
    public override void Use(Item _Item)
    {
        var PlayerWeapons = GameObject.FindGameObjectWithTag("Player");

        if (PlayerWeapons.GetComponent<PlayerController>().WeaponList.Contains(go))
        {
            return;
        }
        else
        {
            go = Instantiate(Model, GameObject.Find("CamHolder").GetComponent<Transform>());
            go.SetActive(false);
            PlayerWeapons.GetComponent<PlayerController>().WeaponList.Add(go);
        }
      
    }
}
