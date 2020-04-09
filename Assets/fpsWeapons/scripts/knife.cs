using UnityEngine;
using System.Collections;

public class knife : MonoBehaviour {
	
	
	public Vector3 normalposition;
	public float speed = 2f;
	public Transform player;


	public AudioSource myAudioSource;
	
	
	public AudioSource fireAudioSource;
	
	public AudioClip[] fireSounds;
	
	public AudioClip readySound;

	public AnimationClip[] fireAnimsA;
	public AnimationClip[] fireAnimsB;
	public AnimationClip hideA;
	public AnimationClip hideB;
	public AnimationClip readytoA;
	public AnimationClip readytoB;
	public AnimationClip switchA;
	public AnimationClip switchB;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;
	private bool isA = true;
	public float fireAnimSpeed = 1.1f;
	public float inaccuracy = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 2f;


	
	public Vector3 retractPos;

	
	private bool retract = false;	

	public Transform rayfirer;
	public Transform grenadethrower;
	private Vector3 wantedrotation;
	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	raycastfire weaponfirer;
	weaponselector inventory;
	public Camera weaponcamera;
	playercontroller playercontrol ;
	Animation myanimation;
	void Awake()
	{
		weaponfirer = rayfirer.GetComponent<raycastfire>();
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();

	}
	void Start () 
	{
		
		myanimation.Stop();
		onstart();

	}

	void Update () 
	{
		
		float step = speed * Time.deltaTime;

		inventory.currentammo = 0;
		//inventory.totalammo = 0;

		float newField = Mathf.Lerp(Camera.main.fieldOfView, normalFOV, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnormalFOV, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
		if (Input.GetButton("ThrowGrenade") && !myanimation.isPlaying && inventory.grenade>0)
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		if (retract) 
		{
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, retractPos,step *2f);
		}



		else if (playercontrol.running)
		{

			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);



		}
		else
		{

			wantedrotation = Vector3.zero;


			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);




		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f );

		if (Input.GetButton("Aim") || 	Input.GetAxis("Aim") > 0.1)
		{
			doswitch();
		}
		if (Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1 )
		{
			fire();
		}
	}
	
	
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();
		retract = false;

		myanimation.Stop();

		if(weaponfirer==null) weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = 1;

		if(inventory==null){ 
			inventory = player.GetComponent<weaponselector>();
			//Init the Current Weapon with ammo value
			inventory.InitCurrentWeaponAmmo(-1);
		}
		inventory.showAIM(false);

		myAudioSource.clip = readySound;
		myAudioSource.loop = false;

		myAudioSource.Play ();
		
		myanimation.Play (readytoA.name);
		isA = true;
	}
	
	void fire()
	{
		if (!myanimation.isPlaying && isA)
		{
						fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			myanimation.clip = fireAnimsA[Random.Range(0,fireAnimsA.Length)];
			myanimation.Play();  
			StartCoroutine(firedelayed(0.3f));
			
		}
		else if (!myanimation.isPlaying)
		{
			fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			myanimation.clip = fireAnimsB[Random.Range(0,fireAnimsB.Length)];
			myanimation.Play();  
			StartCoroutine(firedelayed(0.3f));
		}
	}
	

	
	void doRetract()
	{
		if( isA)

		{
			myanimation.Play(hideA.name);
		}
		else
		{
			myanimation.Play(hideB.name);
		}
	}
	void doNormal()
	{
		
		retract = false;
		onstart();
	}
	IEnumerator firedelayed(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		weaponfirer.fireMelee();
	}
	void doswitch()
	{
		if (isA && !myanimation.isPlaying )
		{
			myanimation.clip = switchB;
			myanimation.Play();  
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			isA = false;
		}
		else if (!myanimation.isPlaying)
		{
			myanimation.clip = switchA;
			myanimation.Play();
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			isA = true;
		}
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
