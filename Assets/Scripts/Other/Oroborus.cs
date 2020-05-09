using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Oroborus : MonoBehaviour
{
    public LayerMask Layer;

    private TextMeshProUGUI Text;

    public GameObject Explosion;

    private float ElapsedTime = 0;
    private float fixedtime = 3f;

    void Start()
    {
        Text = GameObject.Find("Press E to pick").GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, 4, Layer);
        foreach (var hit in collider)
        {
            Text.text = "Press E to Capture Oroborus";
            if (Input.GetKeyDown(KeyCode.E))
            {
                Text.text = "";
                FindObjectOfType<DialogAudioManager>().Play("u survived till now but no longer");
                SceneChangeManager.instance.SceneChangeFunction("AfterStory");
                Instantiate(Explosion, transform.position, Quaternion.identity);
                FindObjectOfType<AudioManager>().Play("PlasmaExplosion");
                Destroy(gameObject,0.01f);
            }
        }
        if (Text.text != "" && ElapsedTime > fixedtime)
        {
            Text.text = "";
            ElapsedTime = 0f;
        }
        else
        {
            ElapsedTime += Time.deltaTime;
        }
    }
}
