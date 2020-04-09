using UnityEngine;
using System.Collections;


public class revolver : MonoBehaviour {

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

	
	
	//public int ammoToReload = 6;

	public AudioClip emptySound;
	public AudioClip fireSound;
	public AudioClip toreloadSound;
	public AudioClip readySound;
	public AudioClip reloadonceSound;
	public AudioClip reloadlastSound;

	public int projectilecount = 1;

	public Transform bullet1;
	public Transform bullet2;
	public Transform bullet3;
	public Transform bullet4;
	public Transform bullet5;
	public Transform bullet6;
	private int bulletactivator = 6;

	private float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;

	public float recoil = 10f;


	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip toreloadAnim;
	public AnimationClip reloadonceAnim;
	public AnimationClip reloadlastAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;

	public AnimationClip reloadB1Anim;
	public AnimationClip reloadB2Anim;
	public AnimationClip reloadB3Anim;
	public AnimationClip reloadB4Anim;
	public AnimationClip reloadB5Anim;
	public AnimationClip reloadB6Anim;
	public GameObject shell;

	public Transform shellPos;

	public float shellejectdelay = 0;
	public int ammo = 200;
	public int currentammo= 20;
	public int clipSize = 20;

	public Transform muzzle;
	public Transform muzzlesmoke;
	public Camera weaponcamera;
	public Transform recoilCamera;
	public float runXrotation = 20;
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

	public Transform rayfirer;
	public Transform player;
	public Transform grenadethrower;
	raycastfire weaponfirer;
	playercontroller playercontrol ;
	weaponselector inventory;
	camerarotate cameracontroller;
	Animation myanimation;
	public Transform meleeweapon;
	void Awake()
	{
		weaponfirer = rayfirer.GetComponent<raycastfire>();
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
		if(Input.GetButton("Reload") ){
			//Debug.Log("RELOAD");
			if (currentammo !=clipSize && ammo >0)
			{

				reload();
			}
		}
		float step = speed * Time.deltaTime;
		weaponfirer.inaccuracy = inaccuracy;
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
				inaccuracy = spreadAim;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
				weaponnextfield = weaponaimFOV;
				nextField = aimFOV;
				recoil = 1f;
			}
			else
			{
				inaccuracy = spreadNormal;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				weaponnextfield = weaponnormalFOV;
				nextField = normalFOV;
				recoil = 5f;
			}

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);

		
		if (bulletactivator == 0)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(false);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 1)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(false);
		}
		else if (bulletactivator == 2)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(false);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
		}
		else if (bulletactivator == 3)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(false);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
		}
		else if (bulletactivator == 4)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(false);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
		}
		else if (bulletactivator == 5)
		{
			bullet1.gameObject.SetActive(false);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
		}
		else if (bulletactivator == 6)
		{
			bullet1.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
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

		if(weaponfirer==null) weaponfirer = rayfirer.GetComponent<raycastfire>();
		weaponfirer.inaccuracy = inaccuracy;
		weaponfirer.damage = damage;
		weaponfirer.range = range;
		weaponfirer.force = force;
		weaponfirer.projectilecount = projectilecount;

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

			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);

			cameracontroller.dorecoil(recoil);

			StartCoroutine(flashthemuzzle());
			weaponfirer.currentmuzzle = muzzle;
			weaponfirer.fire();

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
			StartCoroutine(setreload ());
		} 
	}
	
	void doNormal()
	{
		onstart();
	}

	IEnumerator ejectshell(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		for (int i = 0; i < clipSize - currentammo; i++) {
			Instantiate (shell, shellPos.transform.position, shellPos.transform.rotation);
		}

	}

	IEnumerator setreload()
	{
		//ammoToReload = Mathf.Clamp (ammoToReload, ammoToReload, ammo);
		//reload first
		isreloading = true;
		canaim = false;
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		myAudioSource.clip = toreloadSound;
		myAudioSource.Play();
		
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		myanimation.Play(toreloadAnim.name);
		yield return new WaitForSeconds (myanimation[toreloadAnim.name].length * 0.6f);
		StartCoroutine(ejectshell(shellejectdelay));
		bulletactivator = currentammo;
		
		yield return new WaitForSeconds (myanimation[toreloadAnim.name].length * 0.4f);
		//reloadonce

		while(currentammo != clipSize && ammo >= 1 )
		{
			

			if (bulletactivator == 0)
			{
				myanimation.Play (reloadB5Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB5Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}
			else if (bulletactivator == 1)
			{
				myanimation.Play (reloadB6Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB6Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}
			else if (bulletactivator == 2)
			{
				myanimation.Play (reloadB3Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB3Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}
			else if (bulletactivator == 3)
			{
				myanimation.Play (reloadB4Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB4Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}
			else if (bulletactivator == 4)
			{
				myanimation.Play (reloadB2Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB2Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}
			else if (bulletactivator == 5)
			{
				myanimation.Play (reloadB1Anim.name);
				myAudioSource.clip = reloadonceSound;
				myAudioSource.Play ();
				bulletactivator += 1;
				yield return new WaitForSeconds (myanimation[reloadB1Anim.name].length );
				ammo -= 1;
				currentammo += 1;
				inventory.UpdateCurrentWeaponAmmo(ammo);

			}





			
		} 
		
		//reloadlast
		myAudioSource.clip = reloadlastSound;
		myAudioSource.Play();
		//GetComponent<Animation>()[reloadAnim.name].time = startTime;
		myanimation.Play(reloadlastAnim.name);
		yield return new WaitForSeconds (myanimation[reloadlastAnim.name].length);
		
		playercontrol.canclimb = true;
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;

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

