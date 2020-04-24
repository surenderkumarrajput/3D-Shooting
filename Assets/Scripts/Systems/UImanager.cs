using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public GameObject Inventory;
    public Image DamageImage;

    GameObject playerController;

    public float AlphaMultiplier;

    private float Alpha;
    private Color ImageColorRef;
    void Start()
    {
        Inventory.SetActive(false);
        ImageColorRef = DamageImage.color;
        playerController = GameObject.FindGameObjectWithTag("Player");
        Alpha = DamageImage.color.a;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            Inventory.SetActive(true);
        }
        else
        {
            Inventory.SetActive(false);
        }
         if (playerController.GetComponent<HealthSystems>().CurrentHealth < 30)
         {
            Alpha = 255;
         }
        else
        {
            Alpha -= Alpha*AlphaMultiplier* Time.deltaTime;
        }
        ImageColorRef.a = Alpha;
        DamageImage.color = ImageColorRef;
    }
}
