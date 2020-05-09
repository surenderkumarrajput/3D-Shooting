using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnSystem : MonoBehaviour
{
    public float CurrentStunn;
    public float maxStunn;
    private float ElapsedTime;
    private float FixedTime=5f;
    void Start()
    {
        CurrentStunn = maxStunn;
    }
    public void StunnIncrease()
    {
        if(ElapsedTime>FixedTime)
        {
           CurrentStunn = maxStunn;
           ElapsedTime = 0f;
        }
        else
        {
            ElapsedTime += Time.deltaTime;
        }
        CurrentStunn = Mathf.Clamp(CurrentStunn, 0, maxStunn);
    }
    public float DecreaseStunn(float Amount)
    {
        CurrentStunn -= Amount;
        CurrentStunn = Mathf.Clamp(CurrentStunn, 0, maxStunn);
        return CurrentStunn;
    }
}
