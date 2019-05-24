using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerController : MonoBehaviour {

	public GameObject leftFlicker;
	public GameObject rightFlicker;
	public GameObject noFlicker;

	public GameObject eyeLocation;

	bool showFlicker = false;
	private float hAngle;

	// Use this for initialization
	void Start () {
		StopFlicker ();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.parent.name == "Fove Interface") {
			// this is the first frame, and we should move the flicker canvases on to the left/right eye cameras
			var leftEyeCam = GameObject.Find("FOVE Eye (Left)");
			var rightEyeCam =  GameObject.Find("FOVE Eye (Right)");

			leftFlicker.transform.parent = leftEyeCam.transform;
			rightFlicker.transform.parent = rightEyeCam.transform;
			leftFlicker.GetComponent<Canvas>().worldCamera = leftEyeCam.GetComponent<Camera>();
			rightFlicker.GetComponent<Canvas>().worldCamera = rightEyeCam.GetComponent<Camera>();

			Canvas.ForceUpdateCanvases();
		}

		if (showFlicker) {
			var vectorToEyePosition = eyeLocation.transform.position;
			var rotH = Quaternion.AngleAxis (hAngle, Vector3.up);
			var vectorToAttentionPoint = rotH * Vector3.forward;
			var horAngleBeteenEyesAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x, vectorToAttentionPoint.z), new Vector2 (vectorToEyePosition.x, vectorToEyePosition.z));

			if (horAngleBeteenEyesAndTargetPoint > 0) {
				leftFlicker.SetActive (false);
				rightFlicker.SetActive (true);
			} else {
				leftFlicker.SetActive (true);
				rightFlicker.SetActive (false);
			}
			noFlicker.SetActive (false);
		}
	}


	public void StartFlicker() {
	}


	public void PointTowards(float hAng) {
		showFlicker = true;
		hAngle = hAng;
	}

	public void StopFlicker() {
		showFlicker = false;
		leftFlicker.SetActive (false);
		rightFlicker.SetActive (false);
		noFlicker.SetActive (true);
	}
}
