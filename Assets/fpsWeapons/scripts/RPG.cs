using UnityEngine;
using System.Collections;


public class RPG : MonoBehaviour {

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;
	
	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;
	public float weaponaimFOV  = 20f;
	public float speed = 1f;


	public AudioSource myAudioSource;

	
	public AudioSource fireAudioSource;
	public AudioClip emptySound;
	public AudioClip fireSound;
	
	public AudioClip readySound;
	public AudioClip reloadSound;

	public int ammoToReload = 1;


	public float recoil = 5f;
	public Transform rocket;
	public Transform projectile;
	public Transform projectilePos;
	public Transform muzzle;
	public Transform muzzlesmoke;
	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;

	public int ammo = 200;
	public int currentammo= 20;
	public int clipSize = 20;

	public Camera weaponcamera;
	public Transform recoilCamera;

	private float nextField;
	private float weaponnextfield;

	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	private Vector3 wantedrotation;
	private bool canaim = true;

	private bool canfire = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;
	public Transform meleeweapon;
	public Transform player;


	playercontroller playercontrol ;
	weaponselector inventory;
	camerarotate cameracontroller;
	Animation myanimation;
	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();
		cameracontroller = recoilCamera.GetComponent<camerarotate>();
	}
	void Start()
	{
		clipSize=currentammo;

		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		myanimation.Stop();
		onstart();

	}
	void Update () 
	{
		

		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		inventory.currentammo = currentammo;
		if (Input.GetButton("ThrowGrenade") && !myanimation.isPlaying && inventory.grenade>0 && canfire)
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		if (Input.GetButton("Melee") && !myanimation.isPlaying && canfire) 
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

		if ((Input.GetButton("Aim")|| 	Input.GetAxis("Aim") > 0.1) && canaim && !playercontrol.running)
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
		

		if (currentammo == 0 || currentammo  <= 0 )
		{	
			
			if (ammo <= 0)
			{
				canfire = false;
				canreload = false;
				if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying)
				{
					myAudioSource.PlayOneShot(emptySound);
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

		
		if(!isreloading && canfire){
			if(weaponnextfield == weaponaimFOV)
				inventory.showAIM(false);
			else
				inventory.showAIM(true);
			if ((Input.GetButton("Fire1")  || Input.GetAxis ("Fire1")>0.1)  )
			{
				fire();
			}
		}else{
			inventory.showAIM(false);
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
			inventory.InitCurrentWeaponAmmo(ammo);
		}
		inventory.showAIM(false);

		myanimation.Stop();
		if (isreloading) {
			reload ();
		} 
		else 
		{
			rocket.gameObject.SetActive (true);
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();

			myanimation.Play (readyAnim.name);
			canaim = true;
			canfire = true;
		}
		
	}

	void fire()
	{
		if (!myanimation.isPlaying)
		{
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);
			StartCoroutine(flashthemuzzle());
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);

			cameracontroller.dorecoil(recoil);

			rocket.gameObject.SetActive (false);
			Instantiate(projectile, projectilePos.transform.position,projectilePos.transform.rotation);


			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			myanimation[fireAnim.name].speed = fireAnimSpeed;     
			myanimation.Play(fireAnim.name);
			currentammo -=1;

		

			
			if (currentammo <= 0)
			{
				reload();
			}
		}
	}
	
	void reload()
	{
		if (!myanimation.isPlaying && canreload && !isreloading) {

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
		int oldammo = currentammo;
		isreloading = true;

		canaim = false;
		yield return new WaitForSeconds (waitTime * .2f);
		rocket.gameObject.SetActive (true);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);

		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * .8f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}

	IEnumerator flashthemuzzle()
	{
		ParticleSystem smoke = muzzlesmoke.GetComponent<ParticleSystem>();
		smoke.Emit (1);
		muzzle.transform.localEulerAngles = new Vector3(0f,0f,Random.Range(0f,360f));
		muzzle.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		muzzle.gameObject.SetActive(false);
	}

	IEnumerator setThrowGrenade()
	{
		canfire = false;
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		Animation throwerAnimation = grenadethrower.GetComponent<Animation> ();

		yield return new WaitForSeconds(throwerAnimation.clip.length);
		retract = false;
		canaim = true;
		canfire = true;
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
			canfire = false;
			meleeweapon.gameObject.SetActive (true);
			meleeweapon.gameObject.BroadcastMessage ("melee");
			Animation meleeAnimation = meleeweapon.GetComponent<Animation> ();
			yield return new WaitForSeconds(meleeAnimation.clip.length);
			retract = false;
			canaim = true;
			canfire = true;
			meleeweapon.gameObject.SetActive (false);
		}
	}

}


