using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using System.Collections.Concurrent;
using System;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePlexus : MonoBehaviour
{

	public float maxDistance = 1;
	public bool useParticleWidth = true;
	public float lineWidth = 0.5f;
	public bool useParticleColor = true;
	public Gradient lineColor = default(Gradient);
	public int maxConnections = 5;
	public int maxLines = 100;
	public LineRenderer lineTemplate;
	public bool useJobs = true;

	public static ParticleSystem _particleSystem;
	public static ParticleSystem.Particle[] _particles;
	public static Transform _transform;
	public static Transform _particleSpace;
	public static List<LineRenderer> _lineRenderers;
	public static int curLineRenderer;
	public static int lineRendererCount;
	public static int particleCount;
	public static float maxDistSqr;
	public static LineRenderer template;
	public static int maxLn;
	public static ConcurrentBag<Tuple<Vector3, Vector3>> positions;

	void Start()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		_particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
		_lineRenderers = new List<LineRenderer>();
		_particleSpace = GetSimSpace();
		_transform = transform;
		template = lineTemplate;
		maxLn = maxLines;
		positions = new ConcurrentBag<Tuple<Vector3, Vector3>>();
		for (int i = 0; i < maxLines; i++)
		{
			var lr = GameObject.Instantiate(ParticlePlexus.template, ParticlePlexus._transform, false);
			ParticlePlexus._lineRenderers.Add(lr);
			ParticlePlexus.lineRendererCount++;
		}
	}

	Transform GetSimSpace()
	{
		switch (_particleSystem.main.simulationSpace)
		{
			case ParticleSystemSimulationSpace.World:
				lineTemplate.useWorldSpace = true;
				return _transform;
			case ParticleSystemSimulationSpace.Custom:
				lineTemplate.useWorldSpace = false;
				return _particleSystem.main.customSimulationSpace;
			case ParticleSystemSimulationSpace.Local:
				lineTemplate.useWorldSpace = false;
				return _transform;
			default:
				throw new System.Exception($"Unsupported Space {_particleSystem.main.simulationSpace}");
		}
	}

	void Update()
	{
		if (maxConnections <= 0 || maxLines <= 0)
			return;

		if (Application.isEditor)
		{
			if (_particleSystem.main.maxParticles > _particles.Length)
			{
				_particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
				_particleSpace = GetSimSpace();
			}
		}

		maxDistSqr = maxDistance * maxDistance;
		_particleSystem.GetParticles(_particles);
		particleCount = _particles.Length;

		curLineRenderer = 0;
		lineRendererCount = _lineRenderers.Count;
		if (!useJobs)
		{
			for (int i = 0; i < particleCount; i++)
			{
				if (curLineRenderer >= maxLines)
					break;
				var connections = 0;
				var p1 = _particles[i];
				for (int j = i + 1; j < particleCount; j++)
				{
					if (connections > maxConnections || curLineRenderer >= maxLines)
						break;
					var p2 = _particles[j];
					if (Vector3.SqrMagnitude(p1.position - p2.position) <= maxDistSqr)
					{
						LineRenderer lr;
						if (curLineRenderer == lineRendererCount)
						{
							lr = Instantiate(lineTemplate, _transform, false);
							_lineRenderers.Add(lr);
							lineRendererCount++;
						}

						lr = _lineRenderers[curLineRenderer];
						if (useParticleColor)
						{

							lr.startColor = p1.GetCurrentColor(_particleSystem);
							lr.endColor = p2.GetCurrentColor(_particleSystem);
						}
						else
						{
							lr.colorGradient = lineColor;
						}

						if (useParticleWidth)
						{
							lr.widthCurve = new AnimationCurve(new Keyframe(0, p1.GetCurrentSize(_particleSystem)), new Keyframe(1, p2.GetCurrentSize(_particleSystem)));
							lr.widthMultiplier = 0.5f;
						}
						else
						{
							lr.widthMultiplier = lineWidth;
						}
						lr.enabled = true;
						lr.SetPosition(0, p1.position);
						lr.SetPosition(1, p2.position);

						curLineRenderer++;
						connections++;
					}
				}
			}
		}
		else
		{
			//var job = new PlexusJob();
			//var handle = job.Schedule(particleCount, particleCount / 10);
			//handle.Complete();
			//for (int i = 0; i < positions.Count; i++)
			//{
			//Tuple<Vector3, Vector3> pos;
			//if (positions.TryTake(out pos))
			//{
			//LineRenderer lr;
			//if (i == lineRendererCount)
			//{
			//lr = Instantiate(lineTemplate, _transform, false);
			//_lineRenderers.Add(lr);
			//lineRendererCount++;
			//}
			//lr = _lineRenderers[i];
			//lr.enabled = true;
			//lr.SetPosition(0, pos.Item1);
			//lr.SetPosition(1, pos.Item2);
			//}
			//else
			//i--;
			//}
		}
		for (int i = curLineRenderer; i < lineRendererCount; i++)
			_lineRenderers[i].enabled = false;
	}
}