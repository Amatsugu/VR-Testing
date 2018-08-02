using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DrawEffect : MonoBehaviour
{
	public float maxDrawDist;

	public abstract void SetDrawProgress(float drawProgress);

	public abstract void Fire(float power);
}
