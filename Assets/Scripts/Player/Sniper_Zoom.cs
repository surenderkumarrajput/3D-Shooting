using UnityEngine.UI;
using UnityEngine;

public class Sniper_Zoom : MonoBehaviour
{
    Image crosshair;

    private float FOV;
    public float rate;

    public float Min_FOV;

    public GameObject Object_To_Disable;

    [SerializeField]
    private string Crosshair_Name;
    void Start()
    {
        crosshair = GameObject.Find(Crosshair_Name).GetComponent<Image>();
        crosshair.enabled = false;
        FOV = Camera.main.GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            FOV -= rate;
            Object_To_Disable.SetActive(false);
            crosshair.enabled = true;
        }
        else
        {
            FOV += rate;
            Object_To_Disable.SetActive(true);
            crosshair.enabled = false;
        }
        FOV = Mathf.Clamp(FOV, Min_FOV, 60);
        Camera.main.GetComponent<Camera>().fieldOfView = FOV;
    }
}
