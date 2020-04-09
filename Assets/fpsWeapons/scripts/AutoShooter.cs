using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooter : MonoBehaviour {

	//variables
	public Transform player; 

	public Vector3 normalposition;
	public Vector3 aimposition;	
	public Vector3 retractPos;

	public float aimFOV = 45f;
	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;
	public float weaponaimFOV  = 20f;



	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;

	public AudioClip fireSound;
	public AudioSource fireAudioSource;

	public AnimationClip reloadAnim;
	public AnimationClip reloadClipAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;

	public AnimationClip meleeAnim;
	public AnimationClip throwAnim;
	public AnimationClip ambientAnim;
	private float nextambient = 2f;
	public AudioSource myAudioSource;

	public AudioClip emptySound;
	public AudioClip readySound;
	public AudioClip reloadSound;
	public AudioClip reloadClipSound;
	public AudioClip meleeSound;
	public AudioClip throwSound;


	public int projectilecount = 1;
	private float inaccuracy = 0.02f;
	public float spreadNormal = 0.08f;
	public float spreadAim = 0.02f;
	public float force  = 500f;
	public float damage = 50f;
	public float range = 100f;



	public GameObject shell;
	public Transform shellPos;
	public float shellejectdelay;
	public Transform muzzle;

	public Transform clipShell;



	private float recoil;

	public int ammo = 200;
	public int currentammo= 20;
	public int clipSize = 20;


	public Camera weaponcamera;
	public Transform recoilCamera;
	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;



	//private
	private float nextField;
	private float weaponnextfield;
	private Vector3 wantedrotation;
	private bool canaim = true;

	private bool canfire = true;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	private bool isaiming = false;

	public Transform rayfirer;



	public GameObject grenade;
	public float throwforce;
	public float throwdelay;
	//scriptreference
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
			clipShell.gameObject.SetActive (true);
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			myanimation.Play (readyAnim.name);
			canaim = true;
			canfire = true;
			inventory.showAIM(true);
		}

	}
	void Update ()
	{


		if (!myanimation.isPlaying || myanimation.IsPlaying(ambientAnim.name)) 
		{
			if (!isreloading && canfire) {
				if (weaponnextfield == weaponaimFOV)
					inventory.showAIM (false);
				else
					inventory.showAIM (true);
				if ((Input.GetButton ("Fire1") || Input.GetAxis ("Fire1") > 0.1) && currentammo > 0 ) 
				{

					fire ();


				}
				else if (Input.GetButton("ThrowGrenade") && inventory.grenade>0)
				{

					if(Time.timeSinceLevelLoad>(inventory.lastGrenade+1))
					{
						inventory.lastGrenade=Time.timeSinceLevelLoad;			
						ThrowGrenade();
					}


				}
				else if (Input.GetButton("Melee"))
				{

					StartCoroutine(setMelee (.25f));


				}
				else if(Input.GetButton("Reload") ){
					//Debug.Log("RELOAD");
					if (currentammo !=clipSize && ammo >0)
					{

						reload();
					}
				}
				else if (Time.time > nextambient && !myanimation.IsPlaying(ambientAnim.name) && !isaiming) 
				{

					nextambient = Time.time + Random.Range (4f, 12f);
					myanimation.Play(ambientAnim.name);

				}
				else if (currentammo  <= 0 )
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

					}
					else 
					{
						reload();
					}
				}


			}
			else 
			{
				inventory.showAIM (false);
			}



		} 

		float step = 2 * Time.deltaTime;

		float newField = Mathf.Lerp(Camera.main.fieldOfView, nextField, Time.deltaTime * 2);
		float newfieldweapon = Mathf.Lerp(weaponcamera.fieldOfView, weaponnextfield, Time.deltaTime * 2);
		Camera.main.fieldOfView = newField;
		weaponcamera.fieldOfView = newfieldweapon;
		inventory.currentammo = currentammo;

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

				isaiming = true;
				inaccuracy = spreadAim;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimposition, step);
				weaponnextfield = weaponaimFOV;
				nextField = aimFOV;
				recoil = .5f;
			}
			else
			{


				isaiming = false;
				inaccuracy = spreadNormal;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
				weaponnextfield = weaponnormalFOV;
				nextField = normalFOV;
				recoil = 3f;


			}

		}
		weaponfirer.inaccuracy = inaccuracy;
		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 3f);


	}


	void fire()
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
		StartCoroutine(ejectshell(shellejectdelay));


	}
	void reload()
	{

		if (canreload && !isreloading ) {

			if (currentammo > 0) {
				StartCoroutine (setreload (myanimation [reloadClipAnim.name].length));
				myAudioSource.pitch = 1f;
				myAudioSource.clip = reloadClipSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play ();		
				myanimation.Play (reloadClipAnim.name);
			} 
			else
			{
				myAudioSource.pitch = 1f;
				StartCoroutine (setreload (myanimation [reloadAnim.name].length));
				StartCoroutine (deactivateShell (myanimation [reloadAnim.name].length * 0.5f)); 
				myAudioSource.clip = reloadSound;
				myAudioSource.loop = false;
				myAudioSource.volume = 1;
				myAudioSource.Play ();		
				myanimation.Play (reloadAnim.name);
			}

		}
	}

	void doNormal()
	{
		onstart();
	}

	IEnumerator flashthemuzzle()
	{

		muzzle.transform.localEulerAngles = new Vector3(0f,0f,Random.Range(0f,360f));
		muzzle.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		muzzle.gameObject.SetActive(false);
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
	IEnumerator deactivateShell(float waitTime)
	{
		clipShell.gameObject.SetActive (false);
		yield return new WaitForSeconds(waitTime);
		clipShell.gameObject.SetActive (true);
	}

	IEnumerator setreload(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = currentammo;
		isreloading = true;

		canaim = false;
		yield return new WaitForSeconds (waitTime * .5f);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);

		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * .5f);
		isreloading = false;
		canaim = true;
		inventory.canswitch = true;
		playercontrol.canclimb = true;

	}

	void pickAmmo(int inventoryAmmo){
		ammo=inventoryAmmo;
	}
	IEnumerator setMelee(float waittime)
	{
		myAudioSource.clip = meleeSound;
		myAudioSource.Play();
		myanimation.Play(meleeAnim.name);
		yield return new WaitForSeconds (waittime);
		weaponfirer.fireMelee();
	}
	void ThrowGrenade () 
	{

		if (inventory.grenade>0)
		{

			myAudioSource.clip = throwSound;
			myAudioSource.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource.Play();
			myanimation.Play(throwAnim.name);
			inventory.grenade--;
			StartCoroutine(throwprojectile(throwdelay));
		}
	}

	IEnumerator throwprojectile(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GameObject grenadeInstance = Instantiate(grenade, transform.position+ transform.TransformDirection(new Vector3(-.12f,0.14f,.6f)),transform.rotation) as GameObject;
		yield return null;
		grenadeInstance.GetComponent<Rigidbody>().AddRelativeForce(0f,throwforce/ 4f,throwforce);
	}
	void doRetract()
	{
		myanimation.Play(hideAnim.name);


	}

}