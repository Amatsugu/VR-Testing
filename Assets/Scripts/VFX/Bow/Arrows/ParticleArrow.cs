using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleArrow : ArrowController {

	
	public new ParticleSystem particleSystem;

	private Ray _arrowRay;
	private ParticleSystem.MainModule _mainModule;
	private Transform _thisTransform;
	private Rigidbody _thisRigidbody;

	// Use this for initialization
	void Start () {
		_thisTransform = transform;
		_arrowRay = new Ray(transform.localPosition, Vector3.forward * -1);
		_thisRigidbody = GetComponent<Rigidbody>();
	}
	
	public override void Fire(float power)
	{
		if (_thisTransform == null)
			Start();
		_thisRigidbody.isKinematic = false;
		_thisRigidbody.AddForce(_thisTransform.forward * power * flightSpeed, ForceMode.VelocityChange);
		_thisTransform.parent = null;
	}

	public override void SetDrawProgress(float progress, Vector3 position)
	{
		if (_thisTransform == null)
			Start();
		_mainModule = particleSystem.main;
		//_mainModule.startLifetime = MathUtils.Map(arrowLength * progress, 0, arrowLength, .25f, arrowLength);
		_thisTransform.localPosition = _arrowRay.GetPoint(progress);
	}
}
