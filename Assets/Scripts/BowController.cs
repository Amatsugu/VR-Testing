using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BowController : MonoBehaviour
{

	public Transform arrowAnchor;
	public float minDrawDistance = .2f;
	public float maxDrawDistance = .9f;
	[Range(0, 1)]
	public float drawHapticFequency = .01f;
	public ushort drawHapticIntensity = 1000;
	public AnimationCurve drawHapticCurve;
	public SteamVR_TrackedObject leftHand;
	public SteamVR_TrackedObject rightHand;
	public DrawEffect effect;
	public Transform arrow;

	private int _leftControllerID;
	private int _rightControllerID;
	[SerializeField]
	private float _drawProgress;
	private float _lastDrawHaptic;
	private bool _isDrawing;
	private Transform _thisTransform;
	private LineRenderer _lr;
	private ArrowController _notchedArrow;
	private SteamVR_Controller.Device _leftController;
	private SteamVR_Controller.Device _rightController;
	// Use this for initialization
	void Start ()
	{
		_leftControllerID = (int)OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
		_rightControllerID = (int)OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
		SteamVR_Controller.Input(_leftControllerID).TriggerHapticPulse(1000);
		_thisTransform = transform;
		_lr = GetComponent<LineRenderer>();
		effect.maxDrawDist = maxDrawDistance;
		_leftController = SteamVR_Controller.Input(_leftControllerID);
		_rightController = SteamVR_Controller.Input(_rightControllerID);
	}
	
	// Update is called once per frame
	void Update ()
	{
		var rightTrigger = _rightController.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
		var rightPos = _rightController.transform.pos;
		var leftPos = _thisTransform.position;
		var dir = leftPos - rightPos;
		dir.Normalize();
		var dist = Vector3.Distance(arrowAnchor.transform.position, rightPos);
		Debug.DrawRay(rightPos, dir);
		_drawProgress = dist / maxDrawDistance;
		_drawProgress = (_drawProgress > 1) ? 1 : (_drawProgress < 0) ? 0 : _drawProgress;
		if (rightTrigger >= 0.2f && _drawProgress <= minDrawDistance && !_isDrawing)
			_isDrawing = true;
		if(_isDrawing)
		{
			if (_notchedArrow == null)
			{
				var g = Instantiate(arrow, _thisTransform);
				g.localPosition = arrowAnchor.localPosition;
				g.localRotation = Quaternion.identity;
				_notchedArrow = g.GetComponent<ArrowController>();
			}

			effect.SetDrawProgress(_drawProgress);
			_notchedArrow.SetDrawProgress(_drawProgress, rightPos);
			var intensity = (ushort)(drawHapticCurve.Evaluate(_drawProgress) * drawHapticIntensity);
			_thisTransform.rotation = Quaternion.LookRotation(dir, Vector3.up);
			if(_drawProgress >= drawHapticFequency + _lastDrawHaptic || _drawProgress <= _lastDrawHaptic - drawHapticFequency)
			{
				_rightController.TriggerHapticPulse(intensity);
				_lastDrawHaptic = (drawHapticFequency * (int)(_drawProgress / drawHapticFequency));
			}
			if(_drawProgress == 1 && _lastDrawHaptic != 1)
			{
				_rightController.TriggerHapticPulse((ushort)(2 * drawHapticIntensity));
				_lastDrawHaptic = 1;
			}
			var trajectory = TrajectoryMapper.GetTrajectory(leftPos, 1, _drawProgress * _notchedArrow.flightSpeed * dir);
			_lr.positionCount = trajectory.Length;
			_lr.enabled = true;
			_lr.SetPositions(trajectory);
		}

		if(rightTrigger < .2f)
		{
			if(_isDrawing)
			{
				//Fire
				effect.Fire(_drawProgress);
				_notchedArrow.Fire(_drawProgress);
				_rightController.TriggerHapticPulse((ushort)(_drawProgress * drawHapticIntensity));
				_leftController.TriggerHapticPulse((ushort)(_drawProgress * drawHapticIntensity));
				_notchedArrow = null;
				_lr.enabled = false;
				_drawProgress = 0;
				_lastDrawHaptic = 0;
				_isDrawing = false;
				_thisTransform.localRotation = Quaternion.Euler(90, 0, 0);
			}
		}
	}
}
