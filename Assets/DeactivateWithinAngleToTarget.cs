using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateWithinAngleToTarget : MonoBehaviour {

	public float deactivationAngle;
	public GameObject objectToDeactivate;
	public GameObject eyeLocation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var vectorToEyePosition = eyeLocation.transform.position;
	}
}
