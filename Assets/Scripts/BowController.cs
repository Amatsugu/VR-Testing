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

	private int _leftControllerID;
	private int _rightControllerID;
	[SerializeField]
	private float _drawProgress;
	private float _lastDrawHaptic;
	private bool _isDrawing;

	// Use this for initialization
	void Start ()
	{
		_leftControllerID = (int)leftHand.index; //SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.FarthestLeft, ETrackedDeviceClass.Controller);
		_rightControllerID = (int)rightHand.index; //SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.FarthestRight, ETrackedDeviceClass.Controller);
		SteamVR_Controller.Input(_leftControllerID).TriggerHapticPulse(1000);
	}
	
	// Update is called once per frame
	void Update ()
	{
		var leftController = SteamVR_Controller.Input(_leftControllerID);
		var rightController = SteamVR_Controller.Input(_rightControllerID);
		//transform.position = leftController.transform.pos;
		//transform.rotation = leftController.transform.rot;
		var rightTrigger = rightController.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger).x;
		rightController.Update();
		leftController.Update();
		var rightPos = rightController.transform.pos;
		var leftPos = transform.position;
		var dir = leftPos - rightPos;
		var dist = Vector3.Distance(leftPos, rightPos);
		_drawProgress = dist / maxDrawDistance;
		_drawProgress = (_drawProgress > 1) ? 1 : (_drawProgress < 0) ? 0 : _drawProgress;
		Debug.Log(_drawProgress);
		if (rightTrigger >= 0.2f && _drawProgress <= minDrawDistance && !_isDrawing)
			_isDrawing = true;
		if(_isDrawing)
		{
			var intensity = (ushort)(drawHapticCurve.Evaluate(_drawProgress) * drawHapticIntensity);
			transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
			if(_drawProgress >= drawHapticFequency + _lastDrawHaptic || _drawProgress <= _lastDrawHaptic - drawHapticFequency)
			{
				rightController.TriggerHapticPulse(intensity);
				_lastDrawHaptic = (drawHapticFequency * (int)(_drawProgress / drawHapticFequency));
			}
			if(_drawProgress == 1 && _lastDrawHaptic != 1)
			{
				rightController.TriggerHapticPulse((ushort)(2 * drawHapticIntensity));
				_lastDrawHaptic = 1;
			}
		}

		if(rightTrigger < .2f)
		{
			if(_isDrawing)
			{
				//Fire
				_drawProgress = 0;
				_lastDrawHaptic = 0;
				_isDrawing = false;
			}
			transform.localRotation = Quaternion.identity;
		}
	}
}
