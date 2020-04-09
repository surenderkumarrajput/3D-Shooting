using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class weaponselectorOLD : MonoBehaviour {
	public int currentWeapon = 0;
	//public int numWeapons = 0;

	public Transform[] Weapons;
	public bool[] HaveWeapons;
	public int[] WeaponsAmmo;
	public int grenade;
	public float lastGrenade;
	public float selectInterval = 2f;
	private float nextselect = 2f;

	private int previousWeapon = 0;
	public  AudioClip switchsound;
	public AudioSource myaudioSource;
	public bool canswitch;

	public bool hideweapons = false;
	bool oldhideweapons = false;
	public Text ammotext;
	public Text grenadetext;
	public int currentammo = 10;
	//public int totalammo = 100;

	public GameObject AIM;

	int oldAmmo=-1;
	int oldTotalAmmo=-1;

	void Awake()
	{
		


		for(int i = 0; i < Weapons.Length; i++){
			Weapons[i].gameObject.SetActive(false);
		}

		grenade=10;
		currentWeapon = 0;
		canswitch = true;
		HaveWeapons=new bool[Weapons.Length];
		HaveWeapons[0]=true; // Knife
		if(WeaponsAmmo.Length==0) WeaponsAmmo=new int[Weapons.Length];

		//for test ONLY, to enable all weapons

		for (int i=0;i<Weapons.Length;i++){
			HaveWeapons[i]=true;
		}
	}
	void Start () 
	{
		

		StartCoroutine (selectWeapon (currentWeapon));
	}
	
	// Update is called once per frame
	void Update () 
	{
		bool changeAmmoText=false;
		if(oldAmmo!=currentammo){
			oldAmmo=currentammo;
			changeAmmoText=true;
		}
		if(oldTotalAmmo!=WeaponsAmmo[currentWeapon]){
			oldTotalAmmo=currentammo;
			changeAmmoText=true;
		}
		if(changeAmmoText){
			if(WeaponsAmmo[currentWeapon]==-1)
				ammotext.text="";
			else
				ammotext.text = (currentammo + " / " + WeaponsAmmo[currentWeapon]);
		}
		grenadetext.text = grenade.ToString();

		if(Input.GetAxis("CycleWeapons")>0 && Time.time > nextselect && canswitch 
		   || 
		   (Input.GetButtonDown ("CycleWeapons") && Time.time > nextselect && canswitch && !hideweapons))

		{
			nextselect = Time.time + selectInterval;

			int weaponOK=0;

			for(int i=currentWeapon +1 ; i<Weapons.Length;i++){
				if(HaveWeapons[i]){
					weaponOK=i;
					break;
				}
			}

			previousWeapon = currentWeapon;
			currentWeapon=weaponOK;

			if(currentWeapon!=previousWeapon){
				//Debug.Log("Subtracted");
				playSwithSound();
				StartCoroutine(selectWeapon(currentWeapon));
			}


			// ================Previous Weapon========================
		}
		else if(Input.GetAxis("CycleWeapons")<0 && Time.time > nextselect && canswitch && !hideweapons)
		{
			nextselect = Time.time + selectInterval;

			playSwithSound();
			StartCoroutine(selectWeapon(currentWeapon));

		}
		if (hideweapons!= oldhideweapons)
		{
			if(hideweapons)
			{
				StartCoroutine(hidecurrentWeapon(currentWeapon));
			}
			else
			{
				StartCoroutine(unhidecurrentWeapon(currentWeapon));
			}
		}
	}

	public void playSwithSound(){
		myaudioSource.PlayOneShot(switchsound, 1);
	}

	public void PickAmmo(int weaponNumber,int amountAmmo){
		WeaponsAmmo[weaponNumber]+=amountAmmo;
		if(weaponNumber==currentWeapon)
			Weapons[weaponNumber].gameObject.BroadcastMessage("pickAmmo",WeaponsAmmo[weaponNumber]);
	}

	public void InitCurrentWeaponAmmo(int amountAmmo){
		if(WeaponsAmmo.Length==0)WeaponsAmmo=new int[Weapons.Length];
		//Debug.Log("currentWeapon="+currentWeapon);
		WeaponsAmmo[currentWeapon]+=amountAmmo;
	}

	public void UpdateCurrentWeaponAmmo(int amountAmmo){
		WeaponsAmmo[currentWeapon]=amountAmmo;
	}

	IEnumerator hidecurrentWeapon(int index)
	{
		Weapons[index].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(false);
		oldhideweapons = hideweapons;
	}

	IEnumerator unhidecurrentWeapon(int index)
	{
		yield return new WaitForSeconds (0.15f);
		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);
		oldhideweapons = hideweapons;
	}

	IEnumerator selectWeapon(int index)
	{
		Weapons[previousWeapon].gameObject.BroadcastMessage("doRetract",SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds (0.5f);
		Weapons[previousWeapon].gameObject.SetActive(false);
		Weapons[index].gameObject.SetActive(true);
		Weapons[index].gameObject.BroadcastMessage("doNormal",SendMessageOptions.DontRequireReceiver);
	}

	public void showAIM(bool show){
		if(show)
			AIM.SetActive(true);
		else
			AIM.SetActive(false);
	}

}
