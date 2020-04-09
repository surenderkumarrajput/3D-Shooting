using UnityEngine;
using System.Collections;


public class bow : MonoBehaviour {

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
	public AudioClip aimSound;


	public int ammoToReload = 1;



	public float recoil = 5f;
	public Transform arrow;
	public Transform projectile;
	public Transform projectilePos;

	public AnimationClip aimAnim;
	public AnimationClip unaimAnim;
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
	private bool canfire2 = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;

	public Transform player;
	public Transform meleeweapon;

	playercontroller playercontrol ;
	weaponselector inventory;
	camerarotate cameracontroller;
	Animation myanimation;
	private bool isaiming = false;
	private bool lastaiming;
	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();
		cameracontroller = recoilCamera.GetComponent<camerarotate>();
	}
	void Start()
	{
		clipSize=currentammo;



		lastaiming = isaiming;
		nextField = normalFOV ;
		weaponnextfield = weaponnormalFOV;
		myanimation.Stop();
		onstart();

	}
	void Update () 
	{


		float step = speed * Time.deltaTime;

		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2f);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2f);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		inventory.currentammo = currentammo;
		if (Input.GetButton("ThrowGrenade") && !myanimation.isPlaying && inventory.grenade>0 && canfire2)
		{
			if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1)){
				inventory.lastGrenade=Time.timeSinceLevelLoad;			
				StartCoroutine(setThrowGrenade());
			}
		}
		if (Input.GetButton("Melee") && !myanimation.isPlaying && canfire2) 
		{
			StartCoroutine(setMelee());
		}
		if (retract)
		{
			isaiming = false;
			canaim = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;
		}



		else if (playercontrol.running)
		{
			isaiming = false;

			transform.localPosition = Vector3.MoveTowards(transform.localPosition,runposition, step);
			wantedrotation = new Vector3(runXrotation,runYrotation,0f);
			weaponnextfield = weaponnormalFOV;
			nextField = normalFOV;

		}
		else
		{
			
			wantedrotation = Vector3.zero;
			if (((Input.GetButton("Aim") || Input.GetAxis("Aim") > 0.1)) && canaim && !playercontrol.running)
			{
				isaiming = true;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
				weaponnextfield = weaponaimFOV;
				nextField = aimFOV;
			}
			else
			{
				isaiming = false;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				weaponnextfield = weaponnormalFOV;
				nextField = normalFOV;
			}

		}

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);
		if (isaiming != lastaiming && canaim) 
		{
			StartCoroutine (doaim ());
		}
		if (currentammo == 0 || currentammo  <= 0 )
		{	

			if (ammo <= 0)
			{
				canfire = false;
				canreload = false;
				if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying)
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


		if(!isreloading && canfire){
			
			if ((Input.GetButton("Fire1")  || Input.GetAxis ("Fire1")>0.1)  )
			{
				fire();
			}
		}else{
			inventory.showAIM(false);
		}
		lastaiming = isaiming;

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
			arrow.gameObject.SetActive (true);
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			canfire = false;
			myanimation.Play (readyAnim.name);
			canaim = true;

		}

	}

	void fire()
	{


		if (!myanimation.isPlaying)
		{
			Instantiate(projectile, projectilePos.transform.position,projectilePos.transform.rotation);
			float randomZ = Random.Range (-0.05f,-0.01f);
			//float randomY = Random.Range (-0.1f,0.1f);

			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);

			cameracontroller.dorecoil(recoil);

			arrow.gameObject.SetActive (false);



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
		canfire = false;
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = currentammo;
		isreloading = true;

		canaim = false;
		yield return new WaitForSeconds (waitTime * .6f);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);
		arrow.gameObject.SetActive (true);
		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * .4f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}
	IEnumerator setThrowGrenade()
	{
		canfire2 = false;
		retract = true;
		grenadethrower.gameObject.SetActive(true);
		grenadethrower.gameObject.BroadcastMessage("throwstuff");
		Animation throwerAnimation = grenadethrower.GetComponent<Animation> ();

		yield return new WaitForSeconds(throwerAnimation.clip.length);
		retract = false;
		canaim = true;
		canfire2 = true;
		grenadethrower.gameObject.SetActive(false);
	}

	void pickAmmo(int inventoryAmmo){
		ammo=inventoryAmmo;
	}
	IEnumerator setMelee()
	{
		if (!meleeweapon.gameObject.activeInHierarchy)
		{
			canfire2 = false;
			retract = true;

			meleeweapon.gameObject.SetActive (true);
			meleeweapon.gameObject.BroadcastMessage ("melee");
			Animation meleeAnimation = meleeweapon.GetComponent<Animation> ();
			yield return new WaitForSeconds(meleeAnimation.clip.length);
			retract = false;
			canaim = true;
			canfire2 = true;
			meleeweapon.gameObject.SetActive (false);
		}
	}
	IEnumerator doaim()
	{
		if (isaiming) 
		{
			if (!isreloading && !myanimation.IsPlaying(fireAnim.name)) 
			{
				GetComponent<Animation> ().Play (aimAnim.name);
				myAudioSource.clip = aimSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();	
				yield return new WaitForSeconds (myanimation[aimAnim.name].length);
				canfire = true;
			}


		}
		else 
		{
			if (!isreloading && !myanimation.IsPlaying(fireAnim.name)) {
				GetComponent<Animation> ().Play (unaimAnim.name);
				myAudioSource.clip = aimSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();	
				yield return new WaitForSeconds (myanimation[unaimAnim.name].length);
				canfire = false;
			}


		}
	}

}

