using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamHit : MonoBehaviour
{
	public Transform hitEffect;
	public float beamRadius = 0.25f;
	private Transform _transform;

	private void Start()
	{
		hitEffect.parent = null;
		_transform = transform;
	}
	void Update ()
	{
		RaycastHit hit;
		var ray = new Ray(_transform.position, _transform.forward);
		if (Physics.Raycast(ray, out hit, 10000))
		{
			Physics.CapsuleCast(_transform.position, _transform.position, beamRadius, _transform.forward, out hit, 10000);
			hitEffect.gameObject.SetActive(true);
			//hitEffect.position = _transform.position;
			hitEffect.rotation = _transform.rotation;
			hitEffect.position = ray.GetPoint(Vector3.Distance(_transform.position, hit.point) - .1f);
			//hitEffect.Translate(_transform.forward * (Vector3.Distance(_transform.position, hit.point)));
		}
		else
			hitEffect.gameObject.SetActive(false);
	}
}
