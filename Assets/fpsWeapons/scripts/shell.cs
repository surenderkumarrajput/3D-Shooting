using UnityEngine;
using System.Collections;

public class shell : MonoBehaviour {
	public float  waitTime = 2f;
	public AudioSource myAudioSource;
	public AudioClip[] shellsounds ;
	public LayerMask desiredmask;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, waitTime);	

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter( Collision collision) 
	{
		if (gameObject.layer != desiredmask.value) 
		{
			gameObject.layer = 1 << desiredmask.value;
		}
		if (!myAudioSource.isPlaying)
		{
			
			myAudioSource.clip = shellsounds[Random.Range(0,shellsounds.Length)];
			myAudioSource.Play();
		}
	}

}
