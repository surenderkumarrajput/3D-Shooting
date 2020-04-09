using UnityEngine;
using System.Collections;

public class remotebomb : MonoBehaviour {
	public float waitdetonate = 0.8f;
	public Transform explosion;// Use this for initialization
	public void detonate()
	{
		StartCoroutine(explode(waitdetonate));
	}
	IEnumerator explode (float waittime)
	{
		yield return new WaitForSeconds(waittime);
		Instantiate(explosion, transform.position,transform.rotation);

		Destroy (gameObject);

	}

}
