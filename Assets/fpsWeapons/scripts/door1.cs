using UnityEngine;
using System.Collections;

public class door1 : MonoBehaviour {


	public Transform door;
	public AudioSource myAudioSource;
	public AudioSource myAudioSource2;
	public AudioClip moveSound;
	public AudioClip actionsound;
	public bool canOpen = true;
	public float speed = 2.0f;


	public Vector3 moveposition = Vector3.zero;
	private Vector3 oldposition;
	private Vector3 currentposition;
	private Vector3 startposition;


	private float doorvelocity;
	private bool opendoor = false;

	void Start()
	{
		
		startposition = door.transform.localPosition;
		currentposition = startposition;
		oldposition = currentposition;
	}

	void Update()
	{
		
		currentposition = door.transform.localPosition;
		Vector3 doorvelocityvector = (currentposition - oldposition);
		doorvelocity = doorvelocityvector.magnitude;
		if (opendoor)
		{
			door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, startposition + moveposition,speed * Time.deltaTime);
		}
		else
		{
			door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition,startposition,speed * Time.deltaTime);
		}
		if (doorvelocity == 0f) 
		{
			myAudioSource.Stop();	


		}
		else
		{
			if (!myAudioSource.isPlaying) 
			{
				myAudioSource.loop = true;
				myAudioSource.clip = moveSound;
				myAudioSource.Play ();
			}
		}


		oldposition = currentposition;

	}
	void OnTriggerStay (Collider other) 
	{
		
		if  (other.tag == "Player" && canOpen)
		{
			
			if (Input.GetButtonDown("Action"))//check input
			{
				//boolswitch
				myAudioSource2.PlayOneShot(actionsound);
				opendoor = !opendoor;
								
			}
						
		}


	}
}
