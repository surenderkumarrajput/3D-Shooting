using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class WayPoint : MonoBehaviour
{
    public Image Img;

    public List<Transform> Target = new List<Transform>();

    bool Active = true;
    public TextMeshProUGUI Meter;
    [HideInInspector]
    public int i = 0;

    public Vector3 Offset;

    private void Update()
    {
        float minX = Img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY =Img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(Target[i].transform.position + Offset);

        if (Vector3.Dot((Target[i].transform.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        Img.transform.position = pos;
        Meter.text = ((int)Vector3.Distance(Target[i].transform.position, transform.position)).ToString() + "m";
    }
}