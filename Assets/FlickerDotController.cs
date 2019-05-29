using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerDotController : MonoBehaviour {

	public GameObject modulatingDot;

	public GameObject eyeLocation;

	private RotateToFaceTarget rotateToFaceTarget;

	public DeactivateWithinAngleToTarget deactivator;

	bool showFlicker = false;
	private float hAngle;
	private float vAngle;
	private Vector2 targetSize;

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

	public void PointTowards(AttentionEvent e) {
		showFlicker = true;
		hAngle = e.hAngle;
		vAngle = e.vAngle;
		targetSize = new Vector2(e.width, e.height);
		rotateToFaceTarget.SetTarget(hAngle,vAngle);
		deactivator.UpdateTargetSize(e);
	}

	public void StopFlicker() {
		showFlicker = false;
		modulatingDot.SetActive (false);
	}
}
