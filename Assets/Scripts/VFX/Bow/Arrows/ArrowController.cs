using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArrowController : MonoBehaviour
{
	public float flightSpeed = 10;
	public float arrowLength = 1.5f;
	public float maxDrawDist;

	public abstract void SetDrawProgress(float progress, Vector3 drawPos);

	public abstract void Fire(float power);
}
