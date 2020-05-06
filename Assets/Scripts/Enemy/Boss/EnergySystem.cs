using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public float CurrentEnergy;
    public float MaxEnergy;
    float FixedTime=0.5f, ElapsedTime;
    void Start()
    {
        CurrentEnergy = MaxEnergy;
    }
    public void EnergyIncrease()
    {
        if(ElapsedTime > FixedTime)
        {
            CurrentEnergy += 300 * Time.deltaTime;
            ElapsedTime = 0f;
        }
        else
        {
            ElapsedTime += Time.deltaTime;
        }
        CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, 100);
    }
}
