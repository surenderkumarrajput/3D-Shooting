using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;

    Animator anim;

    public float Wait_Time;
    public string current_scene;

    private void Start()
    {
        if(instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        anim = GetComponent<Animator>();
    }
    public void SceneChangeFunction(string SceneName)
    {
        StartCoroutine(SceneChange(SceneName));
    }
    IEnumerator SceneChange(string SceneName)
    {
        yield return new WaitForSeconds(Wait_Time);
        anim.SetTrigger("EndTransition");
        yield return new WaitForSeconds(5f);
        SceneManager.UnloadSceneAsync(current_scene);
        SceneManager.LoadSceneAsync(SceneName);
    }
    public void SoundPlay(string Soundname)
    {
        FindObjectOfType<AudioManager>().Play(Soundname);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
