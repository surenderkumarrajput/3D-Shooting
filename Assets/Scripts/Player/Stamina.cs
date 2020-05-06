using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public float Max_Stamina;
    public float Current_Stamina;
    private void Start()
    {
        Current_Stamina = Max_Stamina;
    }
    public IEnumerator Recover_Stamina(float Amount)
    {
        while (Current_Stamina!=Max_Stamina)
        {
            Current_Stamina += Amount * Time.deltaTime;
            Current_Stamina = Mathf.Clamp(Current_Stamina, 0, 100);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
