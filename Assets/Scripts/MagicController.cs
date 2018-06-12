using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(ParticleSystem))]
public class MagicController : MonoBehaviour
{
	public SteamVR_Controller.DeviceRelation deviceRelation = SteamVR_Controller.DeviceRelation.FarthestRight;
	public int particleRate = 500;
	private ParticleSystem _ps;
	private ParticleSystem.EmissionModule _em;
	private ParticleSystem.Particle[] _particles;
	private bool _canTrigger = true;
	public int _controllerID = 1;
	void Start()
	{
		_controllerID = SteamVR_Controller.GetDeviceIndex(deviceRelation, ETrackedDeviceClass.Controller);
		SteamVR_Controller.Input(_controllerID).TriggerHapticPulse(1000);
		_ps = GetComponent<ParticleSystem>();
		_em = _ps.emission;
		_particles = new ParticleSystem.Particle[_ps.main.maxParticles];
	}

	void Update()
	{
		if (_ps == null)
			Start();

		var curController = SteamVR_Controller.Input(_controllerID);
		var trigger = curController.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
		if (trigger > .2f)
		{
			_em.enabled = true;
			SteamVR_Controller.Input(_controllerID).TriggerHapticPulse((ushort)(Time.deltaTime * 10000), EVRButtonId.k_EButton_SteamVR_Trigger);
		}
		else
			_em.enabled = false;
		if (curController.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
		{
			_canTrigger = true;
			var vel = transform.forward * 10;
			_ps.GetParticles(_particles);

			for (int i = 0; i < _particles.Length; i++)
			{
				var p = _particles[i];
				if (p.remainingLifetime <= 0 || p.velocity != Vector3.zero)
					continue;
				p.velocity = vel;
				p.remainingLifetime = 20;
				_particles[i] = p;
			}
			_ps.SetParticles(_particles, _particles.Length);
		}
	}
}