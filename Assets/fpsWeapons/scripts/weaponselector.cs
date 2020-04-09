using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weaponselector : MonoBehaviour 
{
	
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

	//private bool inventorycheck = false;
	//public int totalammo = 100;

	public GameObject AIM;

	int oldAmmo=-1;
	int oldTotalAmmo =-1;


	public RectTransform mycanvas;
	private bool inventoryOn;

	public AudioClip uiswitchsound;
	public AudioClip uiswitchsound2;
	public AudioSource myaudio;
	public Color selectColor;
	public Color deselectColor;
	public Color selectedrowColor;

	public Vector2 selectedposition;
	public Vector2 vertical1;
	public Vector2 vertical2;


	public Vector2 horizontal1;
	public Vector2 horizontal2;



	public Sprite[]weaponsprites;

	public GameObject weaponspriteprefab;

	private int currentweapon;
	private int currentrow;



	private GameObject vUI1;
	private GameObject vUI2;


	private GameObject hUI1;
	private GameObject hUI2;

	private GameObject sUI;

	private int hUI1number;
	private int hUI2number;


	private int vUI1number;
	private int vUI2number;
	private int weaponIndex;
	private bool oldinventoryOn;
	private List<List<Sprite>> categorieslist;
	playercontroller playercontrol ;
	void Awake()
	{

		playercontrol = GetComponent<playercontroller>();

		for(int i = 0; i < Weapons.Length; i++){
			Weapons[i].gameObject.SetActive(false);
		}

		grenade=10;
		currentweapon = 0;
		canswitch = true;
		HaveWeapons=new bool[Weapons.Length];
		HaveWeapons[0]=true; // Knife
		if(WeaponsAmmo.Length==0) WeaponsAmmo=new int[Weapons.Length];

		//for test ONLY, to enable all weapons

		for (int i=0;i<Weapons.Length;i++){
			HaveWeapons[i]=true;
		}
	}

	void Start()
	{
		StartCoroutine (selectWeapon (currentweapon));
		inventoryOn = false;
		oldinventoryOn = inventoryOn;
		currentrow = 0;
		hUI1number = currentrow + 1;
		hUI2number= currentrow + 2;
		vUI1number = currentweapon + 1;
		vUI2number= currentweapon + 2;
		//weaponlists , these need to match the weapon transforms
		//meleeweapons 0
		List<Sprite> meleeweaponslist = new List<Sprite>();
		meleeweaponslist.Add(weaponsprites[0]);
		meleeweaponslist.Add(weaponsprites[1]);
		meleeweaponslist.Add(weaponsprites[2]);
		meleeweaponslist.Add(weaponsprites[3]);


		//handguns 1
		List<Sprite> pistolweaponslist = new List<Sprite>();
		pistolweaponslist.Add(weaponsprites[4]);
		pistolweaponslist.Add(weaponsprites[5]);
		pistolweaponslist.Add(weaponsprites[6]);
		pistolweaponslist.Add(weaponsprites[7]);
		pistolweaponslist.Add(weaponsprites[8]);

		//shotguns 2
		List<Sprite> shotgunweaponslist = new List<Sprite>();
		shotgunweaponslist.Add(weaponsprites[9]);
		shotgunweaponslist.Add(weaponsprites[10]);
		shotgunweaponslist.Add(weaponsprites[11]);
		shotgunweaponslist.Add(weaponsprites[12]);

		//automatic weapons 3
		List<Sprite> smgweaponslist = new List<Sprite>();
		smgweaponslist.Add(weaponsprites[13]);
		smgweaponslist.Add(weaponsprites[14]);
		smgweaponslist.Add(weaponsprites[15]);
		smgweaponslist.Add(weaponsprites[16]);
		smgweaponslist.Add(weaponsprites[17]);
		smgweaponslist.Add(weaponsprites[18]);
		smgweaponslist.Add(weaponsprites[32]);
		//longrange 4
		List<Sprite> longrangeweaponslist = new List<Sprite>();
		longrangeweaponslist.Add(weaponsprites[19]);
		longrangeweaponslist.Add(weaponsprites[20]);
		longrangeweaponslist.Add(weaponsprites[21]);
		longrangeweaponslist.Add(weaponsprites[22]);

		//heavy 5
		List<Sprite> heavyweaponslist = new List<Sprite>();
		heavyweaponslist.Add(weaponsprites[23]);
		heavyweaponslist.Add(weaponsprites[24]);
		heavyweaponslist.Add(weaponsprites[25]);
		heavyweaponslist.Add(weaponsprites[26]);
		heavyweaponslist.Add(weaponsprites[27]);
		heavyweaponslist.Add(weaponsprites[28]);
		heavyweaponslist.Add(weaponsprites[29]);

		//explosive 6
		List<Sprite> explosiveweaponslist = new List<Sprite>();
		explosiveweaponslist.Add(weaponsprites[30]);
		explosiveweaponslist.Add(weaponsprites[31]);


		//list of the weaponslists
		categorieslist = new List<List<Sprite>>();
		categorieslist.Add(meleeweaponslist);//0
		categorieslist.Add(pistolweaponslist);//1
		categorieslist.Add(shotgunweaponslist);//2
		categorieslist.Add(smgweaponslist);//3
		categorieslist.Add(longrangeweaponslist);//4
		categorieslist.Add(heavyweaponslist);//5
		categorieslist.Add(explosiveweaponslist);//6

		//Debug.Log(categorieslist[3][3]);

		//vertical gui
		vUI1 = (GameObject)Instantiate(weaponspriteprefab,Vector3.zero,Quaternion.identity);
		vUI1.transform.SetParent(mycanvas.transform,false);
		vUI1.GetComponent<Image>().color = selectedrowColor;
		vUI1.GetComponent<Image>().sprite =  categorieslist[0][1];
		vUI1.GetComponent<RectTransform>().anchoredPosition = vertical1;

		vUI2 = (GameObject)Instantiate(weaponspriteprefab,Vector3.zero,Quaternion.identity);
		vUI2.transform.SetParent(mycanvas.transform,false);
		vUI2.GetComponent<Image>().color = selectedrowColor;
		vUI2.GetComponent<Image>().sprite =  categorieslist[0][2];
		vUI2.GetComponent<RectTransform>().anchoredPosition = vertical2;




		//horizontal gui
		hUI1 = (GameObject)Instantiate(weaponspriteprefab,Vector3.zero,Quaternion.identity);
		hUI1.transform.SetParent(mycanvas.transform,false);
		hUI1.GetComponent<Image>().color = deselectColor;
		hUI1.GetComponent<Image>().sprite = categorieslist[1][0];
		hUI1.GetComponent<RectTransform>().anchoredPosition = horizontal1;

		hUI2 = (GameObject)Instantiate(weaponspriteprefab,Vector3.zero,Quaternion.identity);
		hUI2.transform.SetParent(mycanvas.transform,false);
		hUI2.GetComponent<Image>().color = deselectColor;
		hUI2.GetComponent<Image>().sprite = categorieslist[2][0];
		hUI2.GetComponent<RectTransform>().anchoredPosition = horizontal2;



		//selected gui
		sUI = (GameObject)Instantiate(weaponspriteprefab,Vector3.zero,Quaternion.identity);
		sUI.transform.SetParent(mycanvas.transform,false);

		sUI.GetComponent<Image>().color = selectColor;
		sUI.GetComponent<Image>().sprite = categorieslist[currentrow][currentweapon];
		sUI.GetComponent<RectTransform>().anchoredPosition = selectedposition;

		//unactivate the gui.
		vUI1.SetActive(false);
		vUI2.SetActive(false);


		hUI1.SetActive(false);
		hUI2.SetActive(false);

		sUI.SetActive(false);


	}
	void Update()
	{
		
		bool changeAmmoText=false;
		if(oldAmmo!=currentammo){
			oldAmmo=currentammo;
			changeAmmoText=true;
		}
		if(oldTotalAmmo!=WeaponsAmmo[currentweapon]){
			oldTotalAmmo=currentammo;
			changeAmmoText=true;
		}
		if(changeAmmoText){
			if(WeaponsAmmo[currentweapon]==-1)
				ammotext.text="";
			else
				ammotext.text = (currentammo + " / " + WeaponsAmmo[currentweapon]);
		}
		grenadetext.text = grenade.ToString();

		if (Input.GetButtonDown("Inventory") && canswitch && !playercontrol.climbladder)
		{
			inventoryOn = !inventoryOn;
			myaudio.clip = switchsound;
			myaudio.Play();
		}

		if (inventoryOn!= oldinventoryOn)
		{
			if(inventoryOn)
			{
				StartCoroutine(hidecurrentWeapon(weaponIndex));
			}
			else
			{
				StartCoroutine(selectWeapon(weaponIndex));
			}
		}
		if (inventoryOn)
		{
			playercontrol.respondstoInput = false;
			//activate the gui
			vUI1.SetActive(true);
			vUI2.SetActive(true);


			hUI1.SetActive(true);
			hUI2.SetActive(true);


			sUI.SetActive(true);

			if(Input.GetAxis("Horizontal")<0 && Time.time > nextselect && canswitch && !hideweapons)
			{
				nextselect = Time.time + selectInterval;
				//go leftt
				myaudio.clip = uiswitchsound;
				myaudio.Play();

				currentweapon = 0;
				currentrow += 1;
				hUI1number = currentrow + 1;
				hUI2number = currentrow  + 2;




			}
			else if(Input.GetAxis("Horizontal")>0 && Time.time > nextselect && canswitch && !hideweapons)
			{
				nextselect = Time.time + selectInterval;
				//go right
				myaudio.clip = uiswitchsound;
				myaudio.Play();
				currentweapon = 0;
				currentrow -= 1;
				hUI1number = currentrow + 1;
				hUI2number = currentrow +2;




			}
			if(Input.GetAxis("Vertical")>0 && Time.time > nextselect && canswitch && !hideweapons)
			{
				nextselect = Time.time + selectInterval;
				//go up
				myaudio.clip = uiswitchsound2;
				myaudio.Play();

				currentweapon -= 1;
				vUI1number = currentweapon + 1;
				vUI2number = currentweapon +2;





			}
			else if(Input.GetAxis("Vertical")<0 && Time.time > nextselect && canswitch && !hideweapons)
			{
				nextselect = Time.time + selectInterval;
				//go down
				myaudio.clip = uiswitchsound2;
				myaudio.Play();
				currentweapon += 1;
				vUI1number = currentweapon + 1;
				vUI2number = currentweapon + 2;




			}


			if (currentrow > categorieslist.Count - 1)
			{

				currentrow -= categorieslist.Count ;
			}
			if (currentrow < 0)
			{

				currentrow = categorieslist.Count - 1;
			}

			if (currentweapon < 0)
			{

				currentweapon = categorieslist[currentrow].Count -1;
			}
			if (currentweapon > categorieslist[currentrow].Count-1)
			{

				currentweapon = 0;
			}

			if (hUI1number > categorieslist.Count - 1)
			{
				hUI1number -= categorieslist.Count ;
			}


			if (hUI2number > categorieslist.Count - 1)
			{
				hUI2number -= categorieslist.Count  ;
			}



			if (hUI1number < 0 )
			{
				hUI1number += categorieslist.Count;
			}


			if (hUI2number < 0 )
			{
				hUI2number += categorieslist.Count;
			}


			//...............................................................
			if (vUI1number > categorieslist[currentrow].Count-1)
			{
				vUI1number -= categorieslist[currentrow].Count;
			}

		
			if (vUI2number > categorieslist[currentrow].Count-1)
			{
				vUI2number -= categorieslist[currentrow].Count;
			}




			if (vUI1number < 0 )
			{
				vUI1number += categorieslist[currentrow].Count;
			}


			if (vUI2number < 0 )
			{
				vUI2number += categorieslist[currentrow].Count;
			}

			if ( categorieslist[currentrow].Count <= 2)
			{
				vUI2.SetActive(false);
				if ( categorieslist[currentrow].Count <= 1)
				{
					vUI1.SetActive(true);
				}
			}
			else
			{
				vUI1.SetActive(true);
				vUI2.SetActive(true);
			}

			currentrow = Mathf.Clamp(currentrow,0,categorieslist.Count - 1);
			currentweapon = Mathf.Clamp(currentweapon,0,categorieslist[currentrow].Count-1);
			sUI.GetComponent<Image>().color = selectColor;
			sUI.GetComponent<Image>().sprite = categorieslist[currentrow][currentweapon];

			hUI1number = Mathf.Clamp(hUI1number,0,categorieslist.Count - 1);
			hUI2number = Mathf.Clamp(hUI2number,0,categorieslist.Count - 1);


			hUI1.GetComponent<Image>().sprite =  categorieslist[hUI1number][0];
			hUI2.GetComponent<Image>().sprite =  categorieslist[hUI2number][0];



			vUI1number = Mathf.Clamp(vUI1number,0,categorieslist[currentrow].Count-1);
			vUI2number = Mathf.Clamp(vUI2number,0,categorieslist[currentrow].Count-1);


			vUI1.GetComponent<Image>().sprite =  categorieslist[currentrow][vUI1number];
			vUI2.GetComponent<Image>().sprite =  categorieslist[currentrow][vUI2number];

			weaponIndex = System.Array.IndexOf(weaponsprites,sUI.GetComponent<Image>().sprite);
			oldinventoryOn = inventoryOn;
			if(Input.GetButtonDown("Fire1"))
			{
				
				playSwithSound();
				//StartCoroutine(selectWeapon(weaponIndex));
				inventoryOn = false;


			}



		}
		else
		{
			//disactivate gui
			vUI1.SetActive(false);
			vUI2.SetActive(false);
			hUI1.SetActive(false);
			hUI2.SetActive(false);
			sUI.SetActive(false);
			playercontrol.respondstoInput = true;
			oldinventoryOn = inventoryOn;
		}
		if (hideweapons!= oldhideweapons)
		{
			if(hideweapons)
			{
				StartCoroutine(hidecurrentWeapon(weaponIndex));
			}
			else
			{
				StartCoroutine(unhidecurrentWeapon(weaponIndex));
			}
		}


	}

	public void playSwithSound(){
		myaudioSource.PlayOneShot(switchsound, 1);
	}

	public void PickAmmo(int weaponNumber,int amountAmmo){
		WeaponsAmmo[weaponNumber]+=amountAmmo;
		if(weaponNumber==currentweapon)
			Weapons[weaponNumber].gameObject.BroadcastMessage("pickAmmo",WeaponsAmmo[weaponNumber]);
	}

	public void InitCurrentWeaponAmmo(int amountAmmo){
		if(WeaponsAmmo.Length==0)WeaponsAmmo=new int[Weapons.Length];
		//Debug.Log("currentWeapon="+currentWeapon);
		WeaponsAmmo[currentweapon]+=amountAmmo;
	}

	public void UpdateCurrentWeaponAmmo(int amountAmmo){
		WeaponsAmmo[currentweapon]=amountAmmo;
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



	
