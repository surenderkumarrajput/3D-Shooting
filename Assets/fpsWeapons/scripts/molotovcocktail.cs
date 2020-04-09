using UnityEngine;
using System.Collections;

public class molotovcocktail : MonoBehaviour {
	public Transform explosion;

	public float waitTime = 2.0f;
	public float hitpoints = 50.0f;
	public Transform brokenmodel;

	void update()
	{
		if (hitpoints <= 0.0f) 
		{
			explode();
		}
	}
	void explode() 
	{

		Instantiate(explosion, transform.position,transform.rotation);
		Instantiate(brokenmodel, transform.position,transform.rotation);
		Destroy (gameObject);
	}
	void OnCollisionEnter(Collision collision) 
	{
		
		explode ();
	}
	void Damage (float damage) 
	{
		hitpoints = hitpoints - damage;
	}

}