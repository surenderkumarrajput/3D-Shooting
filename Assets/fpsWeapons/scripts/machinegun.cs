using UnityEngine;
using System.Collections;


public class machinegun : MonoBehaviour {

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
	

	public Transform bullet1;
	public Transform bullet2;
	public Transform bullet3;
	public Transform bullet4;
	public Transform bullet5;
	public Transform bullet6;
	public Transform bullet7;
	public Transform bullet8;
	private Transform nextbullet;

	public int projectilecount = 1;
	private float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;
	public float smoothdamping  = 2f;
	public float recoil = 5f;


	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;
	public AnimationClip reload2Anim;
	public AnimationClip reloadAnim;
	public AnimationClip readyAnim;

	public AnimationClip hideAnim;
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
	public Transform rayfirer;
	public Transform player;
	public Transform meleeweapon;
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
		if(Input.GetButton("Reload") ){
			//Debug.Log("RELOAD");
			if (currentammo !=clipSize && ammo >0)
			{

				reload();
			}
		}

		float step = speed * Time.deltaTime;
		
		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;

		weaponfirer.inaccuracy = inaccuracy;
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
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, retractPos, 5 * Time.deltaTime);
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

		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step* 3f);

		if (currentammo < 8)
		{
			
			if (currentammo == 6)
			{
				bullet8.gameObject.SetActive(false);
				nextbullet = bullet7;
				
			}
			else if (currentammo == 5)
			{
				bullet7.gameObject.SetActive(false);
				bullet8.gameObject.SetActive(false);
				nextbullet = bullet6;
				
			}
			else if (currentammo == 4)
			{
				nextbullet = bullet5;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				
			}
			else if (currentammo == 3)
			{
				nextbullet = bullet4;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				
				
			}
			else if (currentammo == 2)
			{
				nextbullet = bullet3;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				
			}
			else if (currentammo == 1)
			{
				nextbullet = bullet2;
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);
				
			}
			
			else if (currentammo == 0 || currentammo  <= 0 )
			{	
				nextbullet = bullet1;
				
				bullet8.gameObject.SetActive(false);
				bullet7.gameObject.SetActive(false);
				bullet6.gameObject.SetActive(false);
				bullet5.gameObject.SetActive(false);
				bullet4.gameObject.SetActive(false);
				bullet3.gameObject.SetActive(false);
				bullet2.gameObject.SetActive(false);
				bullet1.gameObject.SetActive(false);
				if (ammo <= 0)
				{
					canfire = false;
					canreload = false;
					if (Input.GetButton("Fire1") && canfire || Input.GetAxis ("Fire1")>0.1)
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
			
			
		}
		else
		{
			
			bullet8.gameObject.SetActive(true);
			bullet7.gameObject.SetActive(true);
			bullet6.gameObject.SetActive(true);
			bullet5.gameObject.SetActive(true);
			bullet4.gameObject.SetActive(true);
			bullet3.gameObject.SetActive(true);
			bullet2.gameObject.SetActive(true);
			bullet1.gameObject.SetActive(true);
			nextbullet = null;
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
			if (nextbullet != null)
			{
				nextbullet.gameObject.SetActive (false);
			}
			float randomZ = Random.Range (-0.05f,0.05f);
			//float randomY = Random.Range (-0.1f,0.1f);
			
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y ,transform.localPosition.z + randomZ);
			StartCoroutine(flashthemuzzle());
			cameracontroller.dorecoil(recoil);
			weaponfirer.currentmuzzle = muzzle;
			weaponfirer.fire();
			
			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.2f * Random.value;
			fireAudioSource.Play ();
			myanimation[fireAnim.name].speed = fireAnimSpeed;     
			myanimation.Play(fireAnim.name);
			currentammo -= 1;
			StartCoroutine (ejectshell (shellejectdelay));
			
			if (currentammo <= 0)
			{
				reload();
			}
			
		}
		else 
		{
			if (nextbullet != null)
			{
				
				nextbullet.gameObject.SetActive (true);
			}
		}
	}
	
	void reload()
	{
		if (!myanimation.isPlaying && canreload && !isreloading) {



			if ((currentammo > 6 || currentammo <=0) && ammo >= 6)
			{
				StartCoroutine(setreload (myanimation[reloadAnim.name].length));
				myAudioSource.clip = reloadSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.Play(reloadAnim.name);

			}
			else
			{
				StartCoroutine(setreload (myanimation[reload2Anim.name].length));
				myAudioSource.clip = reload2Sound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play();		

				myanimation.Play(reload2Anim.name);
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
		GameObject shellInstance;
		shellInstance = Instantiate(shell, shellPos.transform.position,shellPos.transform.rotation) as GameObject;
		yield return null;
		shellInstance.GetComponent<Rigidbody>().AddRelativeForce(60,70,0);
		shellInstance.GetComponent<Rigidbody>().AddRelativeTorque(500,20,800);
		shellInstance.transform.localRotation = transform.localRotation * Quaternion.Euler(0,Random.Range(-90f,90f),0);
		
	}

	IEnumerator setreload(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = currentammo;
		isreloading = true;

		canaim = false;
		yield return new WaitForSeconds (waitTime * .35f);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);

		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * .65f);
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

