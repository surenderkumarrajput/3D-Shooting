using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance;
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
    }
    public IEnumerator SceneChange(string SceneName)
    {
        SceneManager.LoadSceneAsync(SceneName);
        yield return null;
    }
}
