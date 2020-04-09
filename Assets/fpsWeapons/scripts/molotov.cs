using UnityEngine;
using System.Collections;


public class molotov : MonoBehaviour {

	public Vector3 normalposition;

	public Vector3 retractPos;


	public float normalFOV  = 65f;
	public float weaponnormalFOV = 32f;

	public float speed = 3f;


	public AudioSource myAudioSource;


	public AudioSource fireAudioSource;

	public AudioClip fireSound;
	public AudioClip emptySound;
	public AudioClip readySound;
	public AnimationClip emptyAnim;
	public AnimationClip reloadAnim;
	public AnimationClip idleAnim;
	public float idleAnimSpeed = .4f;
	public AnimationClip fireAnim;
	public float fireAnimSpeed = 1.1f;
	public GameObject projectile;
	public float throwforce = 200.0f;

	public AnimationClip readyAnim;

	public AnimationClip hideAnim;

	public int ammo = 5;
	public int currentammo= 1;
	public int clipSize = 1;
	public Transform lighterparticles;
	public Transform bottleparticles;



	public Camera weaponcamera;

	public float runXrotation = 20f;
	public float runYrotation = 0f;
	public Vector3 runposition = Vector3.zero;
	private float nextField;
	private float weaponnextfield;
	 
	private Vector3 wantedrotation;


	private bool canfire = true;

	private bool retract = false;	
	private bool isreloading  = false;
	public Transform grenadethrower;
	public Transform handmolotov;
	public Transform player;
	public Transform meleeweapon;
	playercontroller playercontrol ;
	weaponselector inventory;
	Animation myanimation;

	void Awake()
	{
		
		playercontrol = player.GetComponent<playercontroller> ();
		myanimation = GetComponent<Animation> ();

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

		if (retract) {
			transform.localPosition = Vector3.MoveTowards (transform.localPosition, retractPos, step * 2f);
		}
		else 
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalposition, step);
		}


		if (Input.GetButton("Melee")  && myanimation.IsPlaying (idleAnim.name)) 
		{
			StartCoroutine(setMelee());
		}
		if (playercontrol.running)
		{


			wantedrotation = new Vector3(runXrotation,0f,0f);

		}
		else
		{


			wantedrotation = Vector3.zero;

		}
		transform.localRotation = Quaternion.Lerp(transform.localRotation,Quaternion.Euler(wantedrotation),step * 2f);
		if (ammo == 0 && currentammo <= 0)
		{	
			

			canfire = false;

			if ((Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1))
			{
				if (!myAudioSource.isPlaying)
				{
						myAudioSource.PlayOneShot(emptySound);
				}


			}

		}

		else if (Input.GetButton("Fire1") || Input.GetAxis ("Fire1")>0.1  && myanimation.IsPlaying (idleAnim.name) && !isreloading && canfire)
		{

			myanimation.Stop(idleAnim.name);
			fire();
		}
		else if (!myanimation.isPlaying)
		{
			myanimation.CrossFade(idleAnim.name);
		}

	
	

	}

	void doRetract()
	{
		
		if (canfire)
		{
			myanimation.Play(hideAnim.name);
		}
		else
		{
			retract = true;
		}
	}

	void onstart()
	{
		retract = false;
		myAudioSource.Stop();
		fireAudioSource.Stop();
		bottleparticles.gameObject.SetActive(false);	
		lighterparticles.gameObject.SetActive(false);
		if(inventory==null){ 
			inventory = player.GetComponent<weaponselector>();
			//Init the Current Weapon with ammo value
			inventory.InitCurrentWeaponAmmo(ammo);
		}
		inventory.showAIM(false);
		handmolotov.gameObject.SetActive(true);
		myanimation.Stop();



		if (ammo == 0 && currentammo <= 0)
		{
			myanimation.Play (emptyAnim.name);
			canfire = false;
		}
		else
		{
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play ();
			myanimation.Play (readyAnim.name);
			canfire = true;
			myanimation[idleAnim.name].speed = idleAnimSpeed; 
			myanimation.CrossFadeQueued(idleAnim.name);
		}



	}

	void fire()
	{
		if (!myanimation.isPlaying)
		{
			


			fireAudioSource.clip = fireSound;
			fireAudioSource.pitch = 0.9f + 0.1f *Random.value;
			fireAudioSource.Play();
			myanimation[fireAnim.name].speed = fireAnimSpeed;     
			myanimation.Play(fireAnim.name);

			StartCoroutine(throwprojectile(myanimation[fireAnim.name].length));
						



		}
	}


	void doNormal()
	{
		onstart();
	}





	IEnumerator throwprojectile(float waitTime)
	{
		
		lighterparticles.gameObject.SetActive(true);
		yield return new WaitForSeconds(waitTime * 0.3f);
			
		lighterparticles.gameObject.SetActive(false);
		bottleparticles.gameObject.SetActive(true);
		yield return new WaitForSeconds(waitTime * 0.34f);


		handmolotov.gameObject.SetActive(false);

		currentammo -= clipSize;
		inventory.UpdateCurrentWeaponAmmo(ammo);
		GameObject grenadeInstance = Instantiate(projectile, transform.position,transform.rotation) as GameObject;
		yield return null;


		grenadeInstance.GetComponent<Rigidbody>().AddRelativeForce(0f,throwforce/ 4f,throwforce);
		yield return new WaitForSeconds(waitTime * 0.36f);

		handmolotov.gameObject.SetActive(true);
		bottleparticles.gameObject.SetActive(false);	
		if (ammo > 0)
		{
			
			myAudioSource.clip = readySound;
			myAudioSource.loop = false;
			myAudioSource.volume = 1;
			myAudioSource.Play();		
			myanimation.Play(reloadAnim.name);
			playercontrol.canclimb = false;
			inventory.canswitch = false;

			isreloading = true;


			yield return new WaitForSeconds (myanimation[reloadAnim.name].length * .5f);

			currentammo =  0 + (Mathf.Clamp(clipSize ,0,ammo ));
			ammo -= Mathf.Clamp(clipSize, 0,ammo);

			inventory.UpdateCurrentWeaponAmmo(ammo);
			yield return new WaitForSeconds (myanimation[reloadAnim.name].length * .5f);
			isreloading = false;

			inventory.canswitch = true;
			playercontrol.canclimb = true;
		}
		else
		{
			myanimation.Play (emptyAnim.name);
			canfire = false;
		}



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


