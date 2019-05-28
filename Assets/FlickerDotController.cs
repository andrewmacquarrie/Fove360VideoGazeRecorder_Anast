using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerDotController : MonoBehaviour {

	public GameObject modulatingDot;

	public GameObject eyeLocation;

	private RotateToFaceTarget rotateToFaceTarget;

	bool showFlicker = false;
	private float hAngle;
	private float vAngle;

	// Use this for initialization
	void Start () {
		StopFlicker ();
		rotateToFaceTarget = GetComponent<RotateToFaceTarget>();
	}
	
	// Update is called once per frame
	void Update () {
		if (showFlicker) {
			modulatingDot.SetActive(true);
		} else {
			modulatingDot.SetActive(false);
		}
	}

	public void PointTowards(float hAng, float vAng) {
		showFlicker = true;
		hAngle = hAng;
		vAngle = vAng;
		rotateToFaceTarget.SetTarget(hAngle,vAngle);
	}

	public void StopFlicker() {
		showFlicker = false;
		modulatingDot.SetActive (false);
	}
}
