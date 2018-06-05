using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private ParticleSystem _particleSystem;
	private ParticleSystem.Particle[] _particles;
	private Transform _transform;
	private Transform _particleSpace;
	private List<LineRenderer> _lineRenderers;

	void Start ()
	{
		_particleSystem = GetComponent<ParticleSystem>();
		_particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
		_lineRenderers = new List<LineRenderer>();
		_particleSpace = GetSimSpace();
		_transform = transform;
	}

	Transform GetSimSpace()
	{
		switch(_particleSystem.main.simulationSpace)
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

		var maxDistSqr = maxDistance * maxDistance;
		_particleSystem.GetParticles(_particles);
		var particleCount = _particles.Length;

		var curLineRenderer = 0;
		var lineRendererCount = _lineRenderers.Count;

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
					if(curLineRenderer == lineRendererCount)
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

					if(useParticleWidth)
					{
						lr.widthCurve = new AnimationCurve(new Keyframe(0, p1.GetCurrentSize(_particleSystem)), new Keyframe(1, p2.GetCurrentSize(_particleSystem)));
						lr.widthMultiplier = 0.5f;
					}else
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
		for (int i = curLineRenderer; i < lineRendererCount; i++)
			_lineRenderers[i].enabled = false;


	}
}
