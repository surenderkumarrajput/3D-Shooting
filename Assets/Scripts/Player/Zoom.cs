using UnityEngine;
using UnityEngine.UI;

public class Zoom : MonoBehaviour
{
    Image crosshair;

    private float FOV;
    public float rate;

    public float Min_FOV;

    [SerializeField]
    private string Crosshair_Name;
    void Start()
    {
        crosshair = GameObject.Find(Crosshair_Name).GetComponent<Image>();
        crosshair.enabled = false;
        FOV=Camera.main.GetComponent<Camera>().fieldOfView;
    }

    void Update()
    {
         if (Input.GetMouseButton(1))
         {
            FOV -= rate;
            crosshair.enabled = true;
         }
         else
         {
            FOV += rate;
            crosshair.enabled = false;
         }
         FOV = Mathf.Clamp(FOV, Min_FOV, 60);
         Camera.main.GetComponent<Camera>().fieldOfView=FOV;
    }
}
