using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class playercontroller : MonoBehaviour {

	public bool respondstoInput = true;
	public Transform mycamera;
	private Transform reference;

	public float jumpHeight = 2.0f;
	public float jumpinterval = 1.5f;
	private float nextjump = 1.2f;
	private float maxhitpoints = 1000f;
	private float hitpoints = 1000f;
	public float regen = 100f;
	public Text healthtext;
	public AudioClip[] hurtsounds;
	public RawImage painflashtexture;
	private float alpha;
	public Transform recoilCamera;

	public float gravity = 20.0f;
	public float rotatespeed = 4.0f;
	private float speed;
	public float normalspeed = 4.0f;
	public float runspeed = 8.0f;
	public float crouchspeed = 1.0f;
	public float crouchHeight = 1;
	private bool crouching = false;
	public float normalHeight = 2.0f;
	public float camerahighposition = 1.75f;
	public float cameralowposition = 0.9f;
	private float cameranewpositionY;
	private Vector3 cameranewposition;
	private float cameranextposition;
	public float dampTime = 2.0f;



	private float moveAmount;
	public float smoothSpeed = 2.0f;

	private Vector3 forward = Vector3.forward;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 right;

	private float movespeed;
	public Vector3 localvelocity;


	public bool climbladder = false;
	public Quaternion ladderRotation;
	public Vector3 ladderposition;
	public Vector3 ladderforward;
	public Vector3 climbdirection;




	public float climbspeed = 2.0f;


	public bool canclimb = true;
	private Vector3 addVector = Vector3.zero;


	public bool running = false;
	public bool canrun = true;

	public AudioSource myAudioSource;
	public AudioSource myAudioSource2;
	public AudioClip walkloop;
	public AudioClip runloop;
	public AudioClip crouchloop;
	public AudioClip climbloop;
	public AudioClip jumpclip;
	public AudioClip landclip;
	Vector3 targetDirection = Vector3.zero;
	//public Transform playermesh;
	//public Vector3 playermeshNormalPosition;
	//public Vector3 playermeshForwardPosition;
	//public Transform playerskinnedmesh;
	private bool canrun2 = true;
	public bool hideselectedweapon = false;
	Vector3 targetVelocity;
	public float falldamage;
	private float airTime;
	public float falltreshold = 2f;
	private bool prevGrounded;
	public Transform Deadplayer;
	public Transform weaponroot;

	Animator weaponanimator;
	public Transform head;
	Animator headanimator;
	public LayerMask mask;
	CharacterController controller;
	playerrotate rotatescript;
	weaponselector inventory; 
	private float nextcheck;
	void Awake ()
	{
		reference = new GameObject().transform;
		weaponanimator = weaponroot.GetComponent<Animator>();
		headanimator = head.GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		rotatescript = GetComponent<playerrotate>();
		inventory = GetComponent<weaponselector>();	
	}


	void Start () 
	{
		speed = normalspeed;
		painflashtexture.CrossFadeAlpha(0f,0f,true);
		cameranextposition = camerahighposition;

	}
	

	void Update () 
	{
		



		//Animator meshanimator = playermesh.GetComponent<Animator>();
			
		reference.eulerAngles = new Vector3(0, mycamera.eulerAngles.y, 0);
		forward = reference.forward;
		right = new Vector3(forward.z, 0, -forward.x);
		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");



		Vector3 velocity = controller.velocity;
		localvelocity = transform.InverseTransformDirection(velocity);

		bool ismovingforward =localvelocity.z > .5f;



		if (climbladder && !controller.isGrounded && canclimb ) 
		{

			//playermesh.transform.localPosition = Vector3.MoveTowards(playermesh.transform.localPosition,playermeshForwardPosition, Time.deltaTime * 2f);

			inventory.hideweapons = true;
			airTime = 0f;


			crouching = false;
			//playerskinnedmesh.GetComponent<Renderer>().material.SetFloat("_cutoff", 0f)
			if ((localvelocity.magnitude / speed) >= 0.1f)
			{
				myAudioSource.clip = climbloop;
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.loop = true;
					myAudioSource.Play();
				}

			}
			else
			{
				myAudioSource.Stop();
			}


			Vector3 wantedPosition = (ladderposition - transform.position);
			if( wantedPosition.magnitude > 0.05f)
			{
				addVector = wantedPosition.normalized;


			}
			else
			{
				addVector = Vector3.zero;
			}
			//meshanimator.SetBool ("climbladder", true);
			//meshanimator.SetFloat("ver", Input.GetAxis("Vertical"));
			rotatescript.climbing = true;
			rotatescript.ladderforward = ladderforward;
			targetDirection = (ver * climbdirection);
			targetDirection = targetDirection.normalized;
			targetDirection += addVector;

			moveDirection = targetDirection * climbspeed;



		} 
		else 
		{

			inventory.hideweapons = false;

			//playermesh.transform.localPosition = Vector3.MoveTowards(playermesh.transform.localPosition,playermeshNormalPosition, Time.deltaTime * 2f);
			//meshanimator.SetBool ("climbladder", false);
			//playerskinnedmesh.GetComponent<Renderer>().material.SetFloat("_cutoff", 1f);
			rotatescript.climbing = false;

			targetDirection = (hor * right) + (ver * forward);
			targetDirection = targetDirection.normalized;
			targetVelocity = targetDirection;
			if (controller.isGrounded) 
			{
				
				airTime = 0f;
				
				if(Input.GetButtonDown("Crouch")) 
				{ 
					crouchcheck ();

				}
				if (!crouching)
				{   
					//meshanimator.SetBool ("crouch", false);
					canrun = true;
					controller.center = new Vector3(0f,normalHeight / 2f,0f);
					controller.height = normalHeight;
					canrun2 = true;
					cameranextposition = camerahighposition;
					canclimb = true;

				}	
				else if (crouching )
				{
					//meshanimator.SetBool ("crouch", true);
					canrun = false;
					controller.center = new Vector3(0f,crouchHeight / 2f,0f);
					controller.height = crouchHeight;
					canrun2 = false;
					cameranextposition = cameralowposition;
					canclimb = false;

				}
				// Jump
				if (Input.GetButton ("Jump") && Time.time > nextjump)
				{
					nextjump = Time.time + jumpinterval;
					moveDirection.y = jumpHeight;
					myAudioSource.PlayOneShot(jumpclip);
					weaponanimator.SetBool("jump",true);
					headanimator.SetBool("jump",true);
					//meshanimator.SetBool ("jump", true);
					if (crouching)
					{
						crouchcheck ();
					}
				} 
				else 
				{
					weaponanimator.SetBool("jump",false);
					headanimator.SetBool("jump",false);
					//meshanimator.SetBool ("jump", false);
					
				}  

				if ((localvelocity.magnitude/speed) >= 0.1f)
				{




					if (speed == runspeed)
					{
						if (myAudioSource.clip == walkloop || myAudioSource.clip == crouchloop)
						{
							myAudioSource.clip = runloop;
						}
						
					}
					else if (speed == crouchspeed)
					{
						if (myAudioSource.clip == walkloop || myAudioSource.clip == runloop)
						
						{
							myAudioSource.clip = crouchloop;
						}
					}
					else
					{
							
						myAudioSource.clip = walkloop;	
					}

					if (!myAudioSource.isPlaying)
					{
						myAudioSource.loop = true;
						myAudioSource.Play();
					}


				}
				else
				{
					myAudioSource.Pause();
				}
			}
				

			
			else 
			{
				
				airTime += Time.deltaTime;
				moveDirection.y -= (gravity) * Time.deltaTime;
				nextjump = Time.time + jumpinterval;
				myAudioSource.Pause();
				
			}

			if (Input.GetButton ("Fire2") && canrun && canrun2 && ismovingforward) 
			{
				speed = runspeed;
				running = true;
				
			}
			else if(crouching)
			{
				speed = crouchspeed;
				running = false;
			}
			else
			{
				speed = normalspeed;
				running = false;
				
				
			}
			if (respondstoInput)
			{
				targetVelocity *= speed;
				moveDirection.z = targetVelocity.z;
				moveDirection.x = targetVelocity.x;
			}
			else
			{
				
				moveDirection.z = 0;
				moveDirection.x = 0;
			}




		}

		if (hitpoints <= 0)
		{
			//die
			Instantiate(Deadplayer, transform.position, transform.rotation);
			Destroy(gameObject);
		}


			
		cameranewpositionY = Mathf.Lerp(Camera.main.transform.localPosition.y,cameranextposition, Time.deltaTime * 4f);

			

		//meshanimator.SetBool ("grounded", controller.isGrounded);
						
		weaponanimator.SetBool ("grounded", controller.isGrounded);
		weaponanimator.SetFloat("speed",(localvelocity.magnitude), dampTime , .1f);
		headanimator.SetBool ("grounded", controller.isGrounded);
		headanimator.SetFloat("speed",(localvelocity.magnitude), dampTime , .1f);
		//meshanimator.SetFloat("hor",(localvelocity.x/speed) + (Input.GetAxis("Mouse X") /3f), dampTime , 0.2f);
		//meshanimator.SetFloat("ver",(localvelocity.z/ speed), dampTime , 0.8f);


		cameranewposition = new Vector3(Camera.main.transform.localPosition.x,cameranewpositionY,Camera.main.transform.localPosition.z);
		Camera.main.transform.localPosition = cameranewposition;


		controller.Move (moveDirection * Time.deltaTime);
		if (!prevGrounded && controller.isGrounded )
		{
			
			//doland
			if (airTime > falltreshold)
			{
				Damage(falldamage * airTime * 2f);
			}

			if (!myAudioSource.isPlaying && Time.time > nextcheck)
			{
				myAudioSource2.PlayOneShot(landclip);

			}
			nextcheck = Time.time + 0.8f;	
				
		}
		else if (prevGrounded && !controller.isGrounded)
		{
			//dojump


			myAudioSource2.PlayOneShot(jumpclip);

		}
		prevGrounded = controller.isGrounded;	
		if (hitpoints < maxhitpoints)
		hitpoints += regen * Time.deltaTime;
		
		string healthstring = (Mathf.Round(hitpoints/10f)).ToString();
		healthtext.text= (healthstring);
		float alpha = (hitpoints/1000f);
			
		painflashtexture.CrossFadeAlpha(1f - alpha, .5f, false);


	
	}
	void Damage (float damage) 
	{
		camerarotate cameracontroller = recoilCamera.GetComponent<camerarotate>();
		
		cameracontroller.SendMessage("dorecoil", damage/3f,SendMessageOptions.DontRequireReceiver);
		if (!myAudioSource.isPlaying && hitpoints >= 0)
		{

			int n = Random.Range(1,hurtsounds.Length);
			myAudioSource2.clip = hurtsounds[n];
			myAudioSource2.pitch = 0.9f + 0.1f *Random.value;
			myAudioSource2.Play();
			hurtsounds[n] = hurtsounds[0];
			hurtsounds[0] = myAudioSource2.clip;
		}
		//damaged = true;

		hitpoints = hitpoints - damage;
	}
	void crouchcheck()
	{
		//check ceiling!
		Ray heightray = new Ray (transform.position, Vector3.up);
		RaycastHit ceilinghit = new RaycastHit();

		if (!Physics.Raycast (heightray, out ceilinghit, 2.2f, mask)) 
		{
			crouching = !crouching;

		}
	}


}

