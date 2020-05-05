using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Objectives : MonoBehaviour
{
    public static Objectives instance;
    public List<string> Objective = new List<string>();
    public TextMeshProUGUI Objective_Text;

    [HideInInspector]
    public int i = 0;
    private void Start()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Objective_Text == null)
        {
            return;
        }
        else
        {
            Objective_Text.text = "Objective - "+Objective[i];
        }
    }
}
