using UnityEngine;
using System.Collections;

public class TriggerWeapon : MonoBehaviour {
	public int weaponNumber;
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
			inventory.HaveWeapons[weaponNumber]=true;
			inventory.playSwithSound();
			Destroy(gameObject);
		}
	}

}
