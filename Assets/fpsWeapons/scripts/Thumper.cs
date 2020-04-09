using UnityEngine;
using System.Collections;


public class Thumper : MonoBehaviour {

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
	public AudioClip reload2Sound;

	public int ammoToReload = 20;

	public int projectilecount = 1;
	public float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;
	public float smoothdamping  = 2f;
	public float recoil = 5f;


	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AnimationClip reloadAnim;
	public AnimationClip reload2Anim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;
	public GameObject shell;

	public Transform shellPos;

	public float shellejectdelay = 0;
	public int ammo = 200;
	public int currentammo= 2;
	public int clipSize = 2;

	public Transform muzzleLeft;
	public Transform muzzlesmokeleft;

	public Transform muzzleRight;
	public Transform muzzlesmokeright;
	public Transform clipShellLeft;
	public Transform clipShellRight;

	public Camera weaponcamera;
	public Transform recoilCamera;
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
	public Transform meleeweapon;
	public Transform rayfirer;
	public Transform player;

	raycastfire weaponfirer;
	playercontroller playercontrol ;
	weaponselector inventory;
	camerarotate cameracontroller;
	Animation myanimation;
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
		float step = speed * Time.deltaTime;
		if(Input.GetButton("Reload") ){
			//Debug.Log("RELOAD");
			if (currentammo !=clipSize && ammo >0)
			{

				reload();
			}
		}
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
			}
			else
			{
				inaccuracy = spreadNormal;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				weaponnextfield = weaponnormalFOV;
				nextField = normalFOV;
			}

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);
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
			if (currentammo == 1) {	
				weaponfirer.currentmuzzle = muzzleRight;
			} 
			else 
			{
				weaponfirer.currentmuzzle = muzzleLeft;
			}
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

			if (currentammo >=1 )
			{
				StartCoroutine(ejectshell(shellejectdelay)); 
				StartCoroutine(setreload2 (myanimation[reload2Anim.name].length));
				myAudioSource.clip = reload2Sound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.Play(reload2Anim.name);
			}
			else
			{
				StartCoroutine(ejectshell(shellejectdelay)); 
				StartCoroutine(setreload (myanimation[reloadAnim.name].length));
				myAudioSource.clip = reloadSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.Play(reloadAnim.name);
			}
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

	IEnumerator setreload(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		canaim = false;
		isreloading = true;

		yield return new WaitForSeconds (waitTime * 0.5f);

		currentammo += (Mathf.Clamp(clipSize / 2 ,0,ammo ));
		ammo -= 1;
		inventory.UpdateCurrentWeaponAmmo(ammo);

		yield return new WaitForSeconds (waitTime * 0.2f);

		currentammo += (Mathf.Clamp(clipSize /2 ,0,ammo ));
		ammo -= 1;

		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * 0.3f);



		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}
	IEnumerator setreload2(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		canaim = false;
		isreloading = true;

		yield return new WaitForSeconds (waitTime * 0.5f);

		currentammo += (Mathf.Clamp(clipSize / 2 ,0,ammo ));
		ammo -= 1;
		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * 0.5f);



		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}
	IEnumerator flashthemuzzle()
	{
		if (currentammo == 1) {	
			ParticleSystem smoke1 = muzzlesmokeright.GetComponent<ParticleSystem>();
			smoke1.Emit (1);
			muzzleRight.transform.localEulerAngles = new Vector3 (0f, 0f, Random.Range (0f, 360f));
			muzzleRight.gameObject.SetActive (true);
			yield return new WaitForSeconds (0.05f);
			muzzleRight.gameObject.SetActive (false);
		}
		else 
		{
			ParticleSystem smoke2 = muzzlesmokeleft.GetComponent<ParticleSystem>();
			smoke2.Emit (1);
			muzzleLeft.transform.localEulerAngles = new Vector3 (0f, 0f, Random.Range (0f, 360f));
			muzzleLeft.gameObject.SetActive (true);
			yield return new WaitForSeconds (0.05f);
			muzzleLeft.gameObject.SetActive (false);
		}
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
