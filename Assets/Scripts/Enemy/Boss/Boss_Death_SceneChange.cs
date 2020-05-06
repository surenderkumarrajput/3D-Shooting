using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Death_SceneChange : MonoBehaviour
{
    private Boss boss;
    private void Start()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }
    private void Update()
    {
        if(boss!=null)
        {
            if (boss.isDead)
            {
                StartCoroutine(Boss_Death_SceneChange_Function());
            }
        }
    }
    IEnumerator Boss_Death_SceneChange_Function()
    {
        yield return new WaitForSeconds(3f);
        SceneChangeManager.instance.SceneChangeFunction("Win");
    }
}
