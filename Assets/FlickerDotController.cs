using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerDotController : MonoBehaviour {

	public GameObject modulatingDot;

	public GameObject eyeLocation;

	//private RotateToFaceTarget rotateToFaceTarget;

	public DeactivateWithinAngleToTarget deactivator;

	bool showFlicker = false;
	bool angleToTargetSaysIShouldBeActive = true;
	private float hAngle;
	private float vAngle;
	private Vector2 targetSize;

	// Use this for initialization
	void Start () {
		Clear ();
		//rotateToFaceTarget = GetComponent<RotateToFaceTarget>();
	}
	
	// Update is called once per frame
	void Update () {
		if (showFlicker && angleToTargetSaysIShouldBeActive) {
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
		//rotateToFaceTarget.SetTarget(hAngle,vAngle);
		deactivator.UpdateTargetSize(e);
	}

	public void Clear() {
		showFlicker = false;
		modulatingDot.SetActive(false); // this is the root object
		deactivator.Clear(); // need to clear the event from deactivate too for logging and redirecting purposes
	}

	public void ActivationStatusChangedByAngleToTarget(bool shouldActivate){
		angleToTargetSaysIShouldBeActive = shouldActivate;
		// Debug.LogError(shouldActivate);
	}
}
