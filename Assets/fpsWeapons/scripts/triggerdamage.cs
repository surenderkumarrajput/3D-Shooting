using UnityEngine;
using System.Collections;

public class triggerdamage : MonoBehaviour {
	public float damage = 5f;
	public Transform particles;
	public Transform bloodparticles;
	public AudioClip impactsound;
	public AudioSource myAudioSource;

	public bool setOn ;
	void Start()
	{
		
		myAudioSource.Stop();
	}
	void Update()
	{
		if(! setOn)
		{
			
			myAudioSource.Stop();
			ParticleSystem[] particleSystems;
			particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var emis = particle.emission;
				emis.rateOverTime =0;


			}
			ParticleSystem[] bloodparticleSystems;
			bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in bloodparticleSystems)
			{
				var emis = particle.emission;
				emis.rateOverTime =0;

			}
		}
	}
	void OnTriggerStay(Collider other) 
	{
		
		
		if (setOn)
		{
			if (other.gameObject.tag == "flesh")
			{
				ParticleSystem[] bloodparticleSystems;
				bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in bloodparticleSystems)
				{
					var emis = particle.emission;
					emis.rateOverTime =10;
				}
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.clip = impactsound;
					myAudioSource.loop = true;
					myAudioSource.volume = 1;
					myAudioSource.Play ();
				}
				other.transform.SendMessageUpwards ("Damage",damage * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				ParticleSystem[] particleSystems;
				particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particle in particleSystems)
				{
					
					var emis = particle.emission;
					emis.rateOverTime =10;
				}
				if (!myAudioSource.isPlaying)
				{
					myAudioSource.clip = impactsound;
					myAudioSource.loop = true;
					myAudioSource.volume = 1;
					myAudioSource.Play ();
				}
				other.transform.SendMessageUpwards ("Damage",damage * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
			}

		}
		else
		{
			ParticleSystem[] particleSystems;
			particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in particleSystems)
			{
				var emis = particle.emission;
				emis.rateOverTime =0;

			}
			ParticleSystem[] bloodparticleSystems;
			bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particle in bloodparticleSystems)
			{
				var emis = particle.emission;
				emis.rateOverTime =0;

			}
			myAudioSource.Stop ();
		}
	}
	
	void OnTriggerExit(Collider other)
		
	{
		ParticleSystem[] particleSystems;
		particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particle in particleSystems)
		{
			var emis = particle.emission;
			emis.rateOverTime =0;

		}
		ParticleSystem[] bloodparticleSystems;
		bloodparticleSystems = bloodparticles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particle in bloodparticleSystems)
		{
			
			var emis = particle.emission;
			emis.rateOverTime =0;
		}
		myAudioSource.Stop ();
	}
	
}

