using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI text;

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