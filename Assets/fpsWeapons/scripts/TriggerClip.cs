using UnityEngine;
using System.Collections;

public class TriggerClip : MonoBehaviour {
	public int weaponNumber;
	public int ammo;
	weaponselector inventory;
	public GameObject player;
	// Use this for initialization
	void Start () {
		if(player==null) player=GameObject.Find("playerController");
		inventory = player.GetComponent<weaponselector>();
	}

	void OnTriggerEnter (Collider other) 
	{
		//Debug.Log ("tag:"+other.tag);
		if  (other.tag == "Player")
		{
			inventory.PickAmmo(weaponNumber,ammo);
			inventory.playSwithSound();
			Destroy(gameObject);
		}
	}
}
