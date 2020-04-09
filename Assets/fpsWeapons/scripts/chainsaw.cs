using UnityEngine;
using System.Collections;


public class chainsaw : MonoBehaviour {
	

	
	
	public Transform bladecollider;
	public Vector3 normalposition;
	public Vector3 retractPos;
	public float speed = 1f;
	public Transform player;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;

	public AudioSource myAudioSource;
	public Transform blade;
	public float matpanning = -1f;
	public AudioSource fireAudioSource;

	public AudioClip fireSound;
	
	public AudioClip readySound;

	public float smoothdamping  = 2f;
	
	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.0f;
	public AnimationClip idleAnim;
	public float idleAnimSpeed = 0.2f;

	public AnimationClip readyAnim;
	
	public AnimationClip hideAnim;

	public Camera weaponcamera;
	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	private Vector3 wantedrotation;
	private bool retract = false;	
	private bool canfire = true;

	public Transform grenadethrower;
	
	playercontroller playercontrol ;
	weaponselector inventory;
	Animation myanimation;

	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();

	}

	void Start()
	{
		

		myanimation.Stop();
		onstart();
		
	}
	void Update () 
	{
		inventory.currentammo = 0;

		float step = speed * Time.deltaTime;

		float newField = Mathf.Lerp(Camera.main.fieldOfView, normalFOV, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnormalFOV, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
		if (Input.GetButton("ThrowGrenade") && !myanimation.IsPlaying (fireAnim.name))
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		
		if (retract)
		{
			canfire = false;
			bladecollider.GetComponent<triggerdamage>().setOn = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos,step *2f);

		}
		else if (playercontrol.running)
		{
			canfire = true;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);


		}
		else
		{
			canfire = true;
			wantedrotation = Vector3.zero;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);



		}
		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);
		


		if (((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1)) && canfire && !myanimation.IsPlaying (readyAnim.name)&& !myanimation.IsPlaying (hideAnim.name))
		{
			bladecollider.GetComponent<triggerdamage>().setOn = true;
			if (!fireAudioSource.isPlaying)
			{
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play();
			}
			blade.GetComponent<Renderer>().material.SetVector("_panning",new Vector4(0f,matpanning,0f,0f));
			myanimation[fireAnim.name].speed = fireAnimSpeed; 
			
			myanimation.CrossFade(fireAnim.name);
			
			
		}
		else if (!myanimation.IsPlaying (readyAnim.name)&& !myanimation.IsPlaying (hideAnim.name))
		{
			bladecollider.GetComponent<triggerdamage>().setOn = false;
			fireAudioSource.Stop();
			myanimation[idleAnim.name].speed = idleAnimSpeed; 
			
			myanimation.CrossFade(idleAnim.name);
			blade.GetComponent<Renderer>().material.SetVector("_panning",new Vector4(0f,0f,0f,0f));
		}
	}
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();
		retract = false;

		if(inventory==null){ 
			inventory = player.GetComponent<weaponselector>();
			//Init the Current Weapon with ammo value
			inventory.InitCurrentWeaponAmmo(-1);
		}
		inventory.showAIM(false);

		bladecollider.GetComponent<triggerdamage>().setOn = false;
		myanimation.Stop();

			
		myAudioSource.clip = readySound;
		myAudioSource.loop = false;
		myAudioSource.volume = 1;
		myAudioSource.Play ();
			
		myanimation.Play (readyAnim.name);

		canfire = true;

		
	}
	
	
	
	void doRetract()
	{
		myanimation.Play(hideAnim.name);
	}
	void doNormal()
	{
		onstart();
	}
	
	
	IEnumerator setThrowGrenade()
	{
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		yield return new WaitForSeconds(grenadethrower.GetComponent<Animation>()["throwAnim"].length);
		retract = false;

		grenadethrower.gameObject.SetActive(false);
	}
	
}
