using UnityEngine;
using System.Collections;

public class meleeWeapon : MonoBehaviour {
	
	public AudioClip attackSound;
	public AudioSource myAudioSource;
	public AnimationClip[] attackAnims;
	public Transform rayfirer;
	raycastfire weaponfirer;
	Animation myanimation;
	public float firedelay = 0.2f;
	void Awake()
	{

		myanimation = GetComponent<Animation>();
		weaponfirer = rayfirer.GetComponent<raycastfire>();
	}
	void melee()
	{
		if (!myanimation.isPlaying) 
		{
			int n = Random.Range(0,attackAnims.Length);

			myAudioSource.clip = attackSound;
			myAudioSource.pitch = 0.9f + 0.1f * Random.value;
			myAudioSource.Play ();
			myanimation.Play(attackAnims[n].name);
			StartCoroutine(firedelayed(firedelay));
		}
	}
	IEnumerator firedelayed(float waitTime)
	{
		
		yield return new WaitForSeconds (waitTime);
		weaponfirer.fireMelee();
	}

}
