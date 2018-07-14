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
	public TrailRenderer trail;

	public Color fireColor = new Color(1, 0, 0);
	public Color airColor = new Color(1, 1, .9f);
	public Color earthColor = new Color(.4f, .2f, 0);
	public Color voidColor = new Color(1f, 0, .3f);
	public Color waterColor = new Color(0, .5f, 1f);

	private int _lastSelection;
	private Color[] _colors;
	void Start()
	{
		_controllerID = SteamVR_Controller.GetDeviceIndex(deviceRelation, ETrackedDeviceClass.Controller);
		SteamVR_Controller.Input(_controllerID).TriggerHapticPulse(1000);
		_ps = GetComponent<ParticleSystem>();
		_em = _ps.emission;
		_particles = new ParticleSystem.Particle[_ps.main.maxParticles];
		_colors = new[] { voidColor, fireColor, airColor, earthColor, waterColor};
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
		var touch = curController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
		var angle = Angle(touch);
		var length = touch.magnitude;
		angle = Mathf.Round(angle / 90);
		angle = (angle == 4) ? 0 : angle;
		int selection = (length <= .5f) ? 0 : (int)angle + 1;
		Debug.Log($"{angle} ,{length},  {selection}");
		trail.startColor = _colors[selection];
		trail.endColor = _colors[_lastSelection];
		if (selection != _lastSelection)
		{
			curController.TriggerHapticPulse(1000, EVRButtonId.k_EButton_SteamVR_Touchpad);
			_lastSelection = selection;
		}
	}

	public float Angle(Vector2 vector)
	{
		var angle = Mathf.Rad2Deg * Mathf.Atan(vector.y/vector.x);
		if (float.IsNaN(angle))
			return 0;
		if (vector.y < 0 && vector.x >= 0)
			angle = 360 + angle;
		if (vector.x < 0)
			angle = 180 + angle;
		return angle;
	}
}