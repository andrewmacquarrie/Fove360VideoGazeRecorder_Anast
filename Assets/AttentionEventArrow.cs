using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttentionEventArrow : MonoBehaviour {

	private bool hidden;
	private bool angleToTargetSaysIShouldBeActive; // cue can also be hidden if distance to target is reached
	private float hAngle;
	private float vAngle;
	private Vector2 targetSize;
	private bool following;
	private bool stoppedFollowing; // this is a flag to say if we got close enough to the eyes to stop following. Following is activated again when a larger delta is reached
	public float followSpeed = 1f;

	public float stopFollowingAngle = 2f;
	public float startFollowingAngle = 8f;

	public bool isArrow;

	public GameObject eyeLocation;

	public GameObject arrow;

	private RotateToFaceTarget rotateToFaceTarget;

	public DeactivateWithinAngleToTarget deactivator;

	// Use this for initialization
	void Start () {
		hidden = true;
        Clear();
		following = false;
		rotateToFaceTarget = GetComponent<RotateToFaceTarget>();
	}

	// Update is called once per frame
	void Update () {
		if(isArrow){
			var r = arrow.GetComponent<MeshRenderer> ();
			r.enabled = (!hidden && angleToTargetSaysIShouldBeActive);
		} else {
			// its the flicker
		}

		var forward = new Vector3(0,0,1f);

		var vectorToEyePosition = eyeLocation.transform.position;
		
		if(ShouldStopFollowing()){
			stoppedFollowing = true;
		} else if (ShouldStartFollowing()) {
			stoppedFollowing = false;
		}

		if(!stoppedFollowing){
			var horAngleBetweenArrowAndEyes = Vector2.SignedAngle (new Vector2 (transform.position.x,transform.position.z), new Vector2 (vectorToEyePosition.x,vectorToEyePosition.z));
			
			SetHorizontalAngle(horAngleBetweenArrowAndEyes);
			SetVerticalAngle();

			//RotateArrowToFaceTarget();
		}

	}

	private bool ShouldStopFollowing(){
		var angleBetweenEyeAndArrow = Vector3.Angle(eyeLocation.transform.position, arrow.transform.position);
		return angleBetweenEyeAndArrow < stopFollowingAngle;
	}

	private bool ShouldStartFollowing(){
		var angleBetweenEyeAndArrow = Vector3.Angle(eyeLocation.transform.position, arrow.transform.position);
		return angleBetweenEyeAndArrow > startFollowingAngle;
	}

	private void SetVerticalAngle(){
		var currentVerticalAngle = Vector3.SignedAngle (transform.position, Vector3.up, Vector3.up);
		var vectorToEyePosition = eyeLocation.transform.position;
		var angleToEyes = Vector3.SignedAngle (vectorToEyePosition, Vector3.up, Vector3.up);

		var angleBetweenEyesAndArrow = currentVerticalAngle - angleToEyes;

		if(!following){
			transform.RotateAround (new Vector3(0,0,0), transform.forward, angleBetweenEyesAndArrow * -1f);
		} else {
			transform.RotateAround (new Vector3(0,0,0), transform.forward, angleBetweenEyesAndArrow * -1f * followSpeed * Time.deltaTime);
		}
	}

	private void SetHorizontalAngle(float horAngleBetweenArrowAndEyes){
		var up = new Vector3(0,1f,0f);
		if(!following){
			transform.RotateAround (new Vector3(0f,0f,0f), up, horAngleBetweenArrowAndEyes * -1f);
		} else {
			transform.RotateAround (new Vector3(0f,0f,0f), up, horAngleBetweenArrowAndEyes * -1f * followSpeed * Time.deltaTime);
		}

	}

	public void PointTowards(AttentionEvent e) {
		hidden = false;
		following = false;
		hAngle = e.hAngle;
		vAngle = e.vAngle;
		targetSize = new Vector2(e.width, e.height);
		rotateToFaceTarget.SetTarget(hAngle,vAngle);
		deactivator.UpdateTargetSize(e);
		
	}

	public void FollowTo(AttentionEvent e) {
		hidden = false;
		following = true;
		hAngle = e.hAngle;
		vAngle = e.vAngle;
		targetSize = new Vector2(e.width, e.height);
		rotateToFaceTarget.SetTarget(hAngle,vAngle);
		deactivator.UpdateTargetSize(e);
	}

	public void Clear(){
		hidden = true;
		deactivator.Clear(); // need to clear the event from deactivate too for logging and redirecting purposes
	}

	public void ActivationStatusChangedByAngleToTarget(bool shouldActivate){
		angleToTargetSaysIShouldBeActive = shouldActivate;
	}

	public List<string> GetLoggingData(){
		//  "long", "lat", "pixel_x", "pixel_y",
		return new List<string>(new string[] { hAngle.ToString(), vAngle.ToString(), AngleHelperMethods.LongToPixelCoordX(hAngle).ToString(), AngleHelperMethods.LatToPixelCoordY(vAngle).ToString() });
	}

}
