using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HapticsTest : MonoBehaviour
{
	SteamVR_ControllerManager cm;
	SteamVR_Controller controller;
	public uint device = 1;
	public uint axis = 0;
	public float rate = 100;
	public float nextTick = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time >= nextTick)
		{
			OpenVR.System.TriggerHapticPulse(device, axis, char.MaxValue);
			nextTick = Time.time + rate/1000;
		}
	}
}
