using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystems : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;
    private void Start()
    {
        CurrentHealth = MaxHealth;
    }
    public void DecreaseHealth(float Health)
    {
        CurrentHealth -= Health;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }
    public void IncreaseHealth(float Health)
    {
        CurrentHealth += Health;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }
}
