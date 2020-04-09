using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
	
	
public class gamecontrol1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void Update()
	{
		if(Input.GetButtonDown("Quit"))
		{
			Application.Quit ();	
		}
	}
	public void restart()
	{
		SceneManager.LoadScene("corridors");
	}
}
