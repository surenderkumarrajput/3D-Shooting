using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogStory : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Sprite[] BG_Sprites;
    public Image BG;

    [TextArea(1,3)]
    public string[] sentences;

    public float waittime;

    private int index=0;

    public GameObject Continue;

    public string Scenename;

    private void Start()
    {
        StartCoroutine(type());
        Continue.SetActive(false);
    }
    private void Update()
    {
        if (text.text == sentences[index])
        {
            Continue.SetActive(true);
        }
        if(SceneChangeManager.instance.current_scene=="Story")
        {
            if (index == 0)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[0];
            }
            else if (index == 1)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[1];
            }
            else if (index == 2)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[2];
            }
            else if (index == 4)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[3];
            }
            else if (index == 6)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[4];
            }
            else if (index == 7)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[5];
            }
        }
        else if(SceneChangeManager.instance.current_scene == "AfterStory")
        {
            if(index==0)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[0];
            }
            else if(index==4)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[1];
            }
        }
        else if (SceneChangeManager.instance.current_scene == "WinStory")
        {
            if(index==0)
            {
                BG.GetComponent<Image>().sprite = BG_Sprites[0];
            }
        }
    }
    IEnumerator type()
    {
        foreach (char c in sentences[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(waittime);
        }
    }
    public void nextSentence()
    {
        Continue.SetActive(false);
        if (index < sentences.Length - 1)
        {
            FindObjectOfType<AudioManager>().Play("Tap");
            index++;
            text.text = "";
            StartCoroutine(type());
        }
        else
        {
            text.text = "";
            FindObjectOfType<AudioManager>().Play("Tap");
            SceneChangeManager.instance.SceneChangeFunction(Scenename);
        }
    }
}