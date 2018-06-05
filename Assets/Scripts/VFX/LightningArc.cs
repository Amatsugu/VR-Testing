using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class LightningArc : MonoBehaviour {

	public float rate = .1f;
	public float delay = 1;
	public ParticleSystem subEmitter;
	private ParticleSystem.Particle[] _particles;
	private ParticleSystem _particleSystem;
	private float _nextEmit = 0;

	void Start () {
		_particleSystem = GetComponent<ParticleSystem>();
		_particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
	}
	
	void LateUpdate ()
	{
		if (subEmitter == null)
			return;
		if (_particles == null)
			Start();
		_particleSystem.GetParticles(_particles);
		for (int i = 0; i < _particles.Length; i++)
		{
			if(Random.Range(0f,1f) <= rate)
			{
				_particleSystem.TriggerSubEmitter(0, ref _particles[i]);
			}
		}
	}
}
