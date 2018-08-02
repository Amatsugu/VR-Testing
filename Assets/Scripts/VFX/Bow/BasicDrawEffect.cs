using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDrawEffect : DrawEffect
{
	public Transform baseAttach;
	public Transform[] handAttach;
	public float animSpeed = 10f;

	private Ray _ray;
	private float _curPower;

	public void Awake()
	{
		_ray = new Ray(handAttach[0].localPosition, Vector3.forward * -1);
	}

	public override void SetDrawProgress(float drawProgress)
	{
		var scale = MathUtils.Map(drawProgress, 0, 1, .25f, .5f);
		baseAttach.localScale = new Vector3(scale, scale, scale);
		for (int i = 0; i < handAttach.Length; i++)
		{
			var pow = MathUtils.Map(drawProgress, 0, 1, 0, (maxDrawDist / (i + 1)));
			handAttach[i].localPosition = _ray.GetPoint(pow);
			scale = MathUtils.Map(drawProgress, 0, 1, .25f, (.4f / (handAttach.Length - i)));
			handAttach[i].localScale = new Vector3(scale, scale, scale);

		}
	}

	public override void Fire(float power)
	{
		_curPower = power;
		StartCoroutine(FireAnim());
	}

	private IEnumerator FireAnim()
	{
		while(_curPower > 0)
		{
			SetDrawProgress(_curPower -= Time.deltaTime * animSpeed);
			yield return new WaitForEndOfFrame();
		}
		SetDrawProgress(0);
	}
}
