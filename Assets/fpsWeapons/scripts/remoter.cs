using UnityEngine;
using System.Collections;

public class remoter : MonoBehaviour {



	public Vector3 normalposition;

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
	public AudioClip placeSound;

	public AudioClip readySound;
	public AudioClip reloadSound;


	public float smoothdamping  = 2f;


	//public AnimationClip idleAnim;
	//public AnimationClip idle2Anim;
	public AnimationClip placeAnim;
	public AnimationClip fireAnim;
	public AnimationClip readyAnim;
	public AnimationClip hideAnim;


	public LayerMask mask;
	public Transform handbomb;
	public Transform bombprefab;
	public Transform bombposition;
	public int ammo = 200;
	public int currentammo= 20;
	public int clipSize = 20;




	public Camera weaponcamera;

	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	private float nextField;
	private float weaponnextfield;


	private Vector3 wantedrotation;

	private Transform bombInstance = null;
	private bool canreload = true;
	private bool retract = false;	
	private bool isreloading  = false;
	public Transform player;
	public Transform grenadethrower;
	public Transform meleeweapon;
	private bool canfire = true;
	playercontroller playercontrol ;
	weaponselector inventory;
	bool bombready;
	Animation myanimation;
	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller>();
		myanimation = GetComponent<Animation>();

	}
	void Start()
	{
		clipSize=currentammo;
		bombready = false;

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

		if (Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1)
		{

			if (bombready)
			{
				

				fire();
			}
			else
			{
				
				placebomb();
			}

		}

		if (ammo <= 0)
		{

			canreload = false;
			if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1) && !myAudioSource.isPlaying)
			{
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.PlayOneShot(emptySound);
				}
			}

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
		handbomb.gameObject.SetActive(true);
		bombready = false;

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



		}

	}
	void fire()
	{
		if (!myanimation.isPlaying)
		{
			

			StartCoroutine(setfire());




		}
	}
	IEnumerator setfire()
	{
		if (bombInstance != null)
		{
			remotebomb bombscript = bombInstance.GetComponent<remotebomb>();
			bombscript.detonate();
		}

		fireAudioSource.clip = fireSound;
		fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
		fireAudioSource.Play();

		myanimation.Play(fireAnim.name);
		bombready = false;
		yield return new WaitForSeconds (myanimation[fireAnim.name].length);
		if (ammo > 0)
		{
			reload();
		}
	}
	void reload()
	{
		if (canreload) {

			StartCoroutine(setreload (myanimation[readyAnim.name].length));

			myAudioSource.clip = reloadSound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		
			myanimation.Play(readyAnim.name);

		}
	}

	void doNormal()
	{
		onstart();
	}
	void placebomb()
	{
		if (!myanimation.isPlaying)
		{

			StartCoroutine(setbomb(myanimation[placeAnim.name].length));
			fireAudioSource.clip = placeSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();

			myanimation.Play(placeAnim.name);
			currentammo -=1;

		}

	}
	IEnumerator setbomb(float waitTime)
	{
		yield return new WaitForSeconds (waitTime * .5f);
		handbomb.gameObject.SetActive(false);
		//instantiate bomb here


		bombInstance = (Transform)Instantiate(bombprefab,bombposition.transform.position,bombposition.transform.rotation);
		Ray raycheck = new Ray (transform.position , Camera.main.transform.forward);
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast (raycheck, out hit, 2f, mask))
		{
			
			Rigidbody bombrigid = bombInstance.GetComponent<Rigidbody>();

			Destroy(bombrigid);
			bombInstance.transform.parent = hit.transform;
			bombInstance.transform.position = hit.point;
			bombInstance.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
		}


		yield return new WaitForSeconds (waitTime * .5f);
		bombready = true;
	}

	IEnumerator setreload(float waitTime)
	{
		playercontrol.canclimb = false;
		inventory.canswitch = false;
		int oldammo = currentammo;
		handbomb.gameObject.SetActive(true);


		yield return new WaitForSeconds (waitTime * .5f);
		currentammo =  0 + (Mathf.Clamp(clipSize ,clipSize- oldammo,ammo ));
		ammo -= Mathf.Clamp(clipSize, clipSize,ammo);

		inventory.UpdateCurrentWeaponAmmo(ammo);
		yield return new WaitForSeconds (waitTime * .5f);


		inventory.canswitch = true;
		playercontrol.canclimb = true;

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

			canfire = true;
			meleeweapon.gameObject.SetActive (false);
		}
	}

}
