using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;

struct PlexusJob : IJobParallelFor
{
	public void Execute(int i)
	{
		if (ParticlePlexus.curLineRenderer >= ParticlePlexus.maxLn)
			return;
		var connections = 0;
		var p1 = ParticlePlexus._particles[i];
		for (int j = i + 1; j < ParticlePlexus.particleCount; j++)
		{
			if (ParticlePlexus.curLineRenderer >= ParticlePlexus.maxLn)
				break;
			var p2 = ParticlePlexus._particles[j];
			if (Vector3.SqrMagnitude(p1.position - p2.position) <= ParticlePlexus.maxDistSqr)
			{
				/*LineRenderer lr;
				

				lr = ParticlePlexus._lineRenderers[ParticlePlexus.curLineRenderer];*/
				//if (useParticleColor)
				//{

				//lr.startColor = p1.GetCurrentColor(ParticlePlexus._particleSystem);
				//lr.endColor = p2.GetCurrentColor(ParticlePlexus._particleSystem);
				//}
				//else
				//{
				//lr.colorGradient = lineColor;
				//}

				//if (useParticleWidth)
				//{
				//lr.widthCurve = new AnimationCurve(new Keyframe(0, p1.GetCurrentSize(ParticlePlexus._particleSystem)), new Keyframe(1, p2.GetCurrentSize(ParticlePlexus._particleSystem)));
				//lr.widthMultiplier = 0.5f;
				//}
				//else
				//{
				//lr.widthMultiplier = lineWidth;
				//}
				/*lr.enabled = true;
				lr.SetPosition(0, p1.position);
				lr.SetPosition(1, p2.position);
				*/
				ParticlePlexus.positions.Add(new Tuple<Vector3, Vector3>(p1.position, p2.position));
				ParticlePlexus.curLineRenderer++;
				connections++;
			}
		}
	}
}