using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;
    public GameObject Loading_Screen;
    public TextMeshProUGUI Loading_Percent;
    public Image Loading_Bar;
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
        Loading_Screen.SetActive(false);
        Loading_Bar.fillAmount = 0f;
    }
    public void SceneChangeFunction(string SceneName)
    {
        StartCoroutine(SceneChange(SceneName));
    }
    IEnumerator SceneChange(string SceneName)
    {
        Loading_Screen.SetActive(true);
        AsyncOperation asyncOperation=SceneManager.LoadSceneAsync(SceneName,LoadSceneMode.Single);
        while(!asyncOperation.isDone)
        {
            float Progress=Mathf.Clamp01(asyncOperation.progress/0.9f);
            Loading_Bar.fillAmount += Progress;
            Loading_Percent.text = Progress * 100 + "%";
            yield return null;
        }
        Loading_Screen.SetActive(false);
    }
}
