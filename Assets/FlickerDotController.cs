using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerDotController : MonoBehaviour {

	public GameObject modulatingDot;

	public GameObject eyeLocation;

	bool showFlicker = false;
	private float hAngle;

	// Use this for initialization
	void Start () {
		StopFlicker ();
	}
	
	// Update is called once per frame
	void Update () {
		if (showFlicker) {
			modulatingDot.SetActive(true);
		} else {
			modulatingDot.SetActive(false);
		}
	}

	public void PointTowards(float hAng) {
		showFlicker = true;
		hAngle = hAng;
	}

	public void StopFlicker() {
		showFlicker = false;
		modulatingDot.SetActive (false);
	}
}
