using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public GameObject Inventory;
    private GameObject playerController;
    public GameObject PauseMenu;

    public Image DamageImage;

    public float AlphaMultiplier;
    float ElapsedTime=0f;
    float fixedTime = 1f;
    private float Alpha;
    private Color ImageColorRef;
    void Start()
    {
        Inventory.SetActive(false);
        ImageColorRef = DamageImage.color;
        if(GameObject.FindGameObjectWithTag("Player")==null)
        {
            return;
        }
        else
        {
            playerController = GameObject.FindGameObjectWithTag("Player");
        }
        Alpha = DamageImage.color.a;
        PauseMenu.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = PauseMenu.activeSelf;
            if(isActive)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 0f;
            }
            PauseMenu.SetActive(!isActive);
        }
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            return;
        }
        else
        {
            if (playerController.GetComponent<HealthSystems>().CurrentHealth < 30)
            {
                Alpha = 255;
                if(ElapsedTime>fixedTime)
                {
                    FindObjectOfType<AudioManager>().Play("HeartBeat");
                    ElapsedTime = 0f;
                }
                else
                {
                    ElapsedTime += Time.deltaTime;
                }
            }
            else
            {
                Alpha -= Alpha * AlphaMultiplier * Time.deltaTime;
            }
        }
        ImageColorRef.a = Alpha;
        DamageImage.color = ImageColorRef;
    }
    public void ResumeButton(GameObject Go)
    {
        Go.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        SceneChangeManager.instance.SceneChangeFunction("Main");
        Time.timeScale = 1f;
    }
}
