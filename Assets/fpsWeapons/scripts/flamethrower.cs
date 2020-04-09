using UnityEngine;
using System.Collections;


public class flamethrower : MonoBehaviour {

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;
	
	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;
	public float weaponaimFOV  = 20f;
	public float speed = 2f;


	public AudioSource myAudioSource;

	
	public AudioSource fireAudioSource;
	public AudioClip emptySound;
	public AudioClip fireSound;
	public Transform flames;
	public AudioClip readySound;
	public AudioClip reloadSound;
	
	


	public float ammo = 200f;
	public float currentammo= 40f;

	public int clipSize = 40;
	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;



	public Camera weaponcamera;

	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	private float nextField;
	private float weaponnextfield;


	private Vector3 wantedrotation;
	private bool canaim = true;

	private bool canfire = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;
	private AnimationCurve curve;
	public Transform player;

	//raycastfire weaponfirer;
	playercontroller playercontrol ;
	weaponselector inventory;
	public Transform meleeweapon;
	//camerarotate cameracontroller;
	Animation myanimation;
	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();

	}
	void Start()
	{
		//weaponfirer = rayfirer.GetComponent<raycastfire>();
		playercontrol = player.GetComponent<playercontroller>();
		//cameracontroller = recoilCamera.GetComponent<camerarotate>();

		curve = new AnimationCurve ();
		curve.AddKey(0.0f,0.1f);
		curve.AddKey(0.75f,1.0f);
		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		myanimation.Stop();
		onstart();

	}
	void Update () 
	{
		

		float step = speed * Time.deltaTime;
		if(Input.GetButton("Reload") ){
			//Debug.Log("RELOAD");
			if (currentammo !=clipSize && Mathf.RoundToInt(ammo) >0)
			{

				reload();
			}
		}
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		inventory.currentammo = Mathf.RoundToInt(currentammo);
		if (Input.GetButton("ThrowGrenade") && myanimation[fireAnim.name].speed == 0f)
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		if (Input.GetButton("Melee")  && myanimation[fireAnim.name].speed == 0f) 
		{
			StartCoroutine(setMelee());
		}
		if (retract)
		{
			canfire = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, step *2f);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}


		else if (playercontrol.running)
		{
			canfire = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		else
		{
			canfire = true;
			wantedrotation = Vector3.zero;
			if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim && !playercontrol.running)
			{

				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
				weaponnextfield = weaponaimFOV;
				nextField = aimFOV;
			}
			else
			{

				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				weaponnextfield = weaponnormalFOV;
				nextField = normalFOV;
			}

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);

		if (currentammo  <= 0f )
		{	
			
			if (ammo <= 0f)
			{
				canfire = false;
				canreload = false;
				if ((Input.GetButton("Fire1") && canfire || Input.GetAxis ("Fire1")>0.1) )
				{
					if (!myAudioSource.isPlaying)
					{
						myAudioSource.PlayOneShot(emptySound);
					}
				}
				else
				{
					canreload = true;
				}
			}
			else 
			{
				reload();
			}
			
			
		}
		if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && canfire && !isreloading && !myanimation.IsPlaying (readyAnim.name)&& !myanimation.IsPlaying (hideAnim.name))
		{
			
			currentammo -= 5f * Time.deltaTime;
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rateOverTime= 50f;
			}
			if (!fireAudioSource.isPlaying)
			{
				fireAudioSource.clip = fireSound;
				fireAudioSource.loop = true;
				fireAudioSource.Play();
			}

			myanimation[fireAnim.name].speed = fireAnimSpeed; 

			myanimation.Play(fireAnim.name);


		}
		else
		{
			ParticleSystem[] particleSystems;
			particleSystems = flames.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var em = particle.emission;
				em.rateOverTime= 0f;
			}
			fireAudioSource.Stop();
			myanimation[fireAnim.name].speed = 0f;

		}
		
		
	}
	
	void doRetract()
	{
		myanimation.Play(hideAnim.name);
	}
	
	void onstart()
	{
		myAudioSource.Stop();
		fireAudioSource.Stop();

		if(inventory==null){ 
			inventory = player.GetComponent<weaponselector>();
			//Init the Current Weapon with ammo value
			inventory.InitCurrentWeaponAmmo((int)ammo);
		}

		myanimation.Stop();
		if (isreloading) {
			reload ();
		} 
		else 
		{
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			myanimation.Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
		
	}

	
	void reload()
	{
		if (canreload && !isreloading) {
			StartCoroutine(setreload (myanimation[reloadAnim.name].length));

			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		
			myanimation.Play(reloadAnim.name);

		} 
	}
	
	

	void doNormal()
	{
		onstart();
	}



	IEnumerator setreload(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = Mathf.RoundToInt(currentammo);
		isreloading = true;

		canaim = false;
		yield return new WaitForSeconds (waitTime * .5f);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);

		inventory.UpdateCurrentWeaponAmmo(Mathf.RoundToInt(ammo));
		yield return new WaitForSeconds (waitTime * .5f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}

	IEnumerator setThrowGrenade()
	{
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		Animation throwerAnimation = grenadethrower.GetComponent<Animation> ();

		yield return new WaitForSeconds(throwerAnimation.clip.length);
		retract = false;
		canaim = true;
		grenadethrower.gameObject.SetActive(false);
	}

	void pickAmmo(int inventoryAmmo){
		ammo=inventoryAmmo;
	}
	IEnumerator setMelee()
	{
		if (!meleeweapon.gameObject.activeInHierarchy)
		{

			retract = true;
			meleeweapon.gameObject.SetActive (true);
			meleeweapon.gameObject.BroadcastMessage ("melee");
			Animation meleeAnimation = meleeweapon.GetComponent<Animation> ();
			yield return new WaitForSeconds(meleeAnimation.clip.length);
			retract = false;
			canaim = true;
			meleeweapon.gameObject.SetActive (false);
		}
	}

}
