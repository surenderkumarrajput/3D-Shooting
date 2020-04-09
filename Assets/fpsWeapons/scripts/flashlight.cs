using UnityEngine;
using System.Collections;

public class flashlight : MonoBehaviour {
	public Vector3 normalposition;
	public float speed = 2f;
	public Transform player;


	public AudioSource myAudioSource;


	public AudioSource fireAudioSource;

	public AudioClip[] fireSounds;

	public AudioClip readySound;


	public AnimationClip[] fireAnims;

	public AnimationClip idleAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;

	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;

	public float idleAnimSpeed = .4f;
	public float inaccuracy = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 2f;



	public Vector3 retractPos;


	private bool retract = false;	

	public Camera weaponcamera;

	public Transform rayfirer;
	public Transform grenadethrower;
	private Vector3 wantedrotation;
	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	raycastfire weaponfirer;
	weaponselector inventory;
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
	
	// Update is called once per frame
	void Update () 
	{

		float step = speed * Time.deltaTime;
		float newField = Mathf.Lerp(Camera.main.fieldOfView, normalFOV , Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnormalFOV, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
		inventory.currentammo = 0;
		//inventory.totalammo = 0;

		if (Input.GetButton("ThrowGrenade") && myanimation.IsPlaying (idleAnim.name) && inventory.grenade>0 )
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		if (retract)
		{
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step* 2f);


		}


		else if (playercontrol.running )
		{
			
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);



		}
		else
		{
			
			wantedrotation = Vector3.zero;

				
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				
				


		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);

		if (Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1  && myanimation.IsPlaying (idleAnim.name))
		{
			
			myanimation.Stop(idleAnim.name);

			fire();

		}
		else if (!myanimation.isPlaying)
		{
			
		
			myanimation[idleAnim.name].speed = idleAnimSpeed; 
			myanimation.CrossFade(idleAnim.name);

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

		if(inventory==null) inventory = player.GetComponent<weaponselector>();
		inventory.showAIM(false);
		myanimation.Play (readyAnim.name);
		myAudioSource.clip = readySound;
		myAudioSource.loop = false;
		//myAudioSource.volume = 1;
		myAudioSource.Play ();

	}

	void fire()
	{
		if (!myanimation.isPlaying)
		{
			fireAudioSource.clip = fireSounds[Random.Range(0,fireSounds.Length)];
			fireAudioSource.pitch = 0.98f + 0.1f *Random.value;
			fireAudioSource.Play();
			myanimation.clip = fireAnims[Random.Range(0,fireAnims.Length)];
			myanimation.Play();  
			StartCoroutine(firedelayed(0.3f));

		}

	}



	void doRetract()
	{
		
		myanimation.Play(hideAnim.name);

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
