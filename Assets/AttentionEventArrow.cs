using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AttentionEventArrow : MonoBehaviour {

	private bool hidden;
	private float hAngle;
	private float vAngle;
	private bool following;
	private bool stoppedFollowing; // this is a flag to say if we got close enough to the eyes to stop following. Following is activated again when a larger delta is reached
	public float followSpeed = 1f;

	public float stopFollowingAngle = 2f;
	public float startFollowingAngle = 8f;

	public GameObject eyeLocation;

	public GameObject arrow;

	// Use this for initialization
	void Start () {
		hidden = true;
		following = false;
	}

	// Update is called once per frame
	void Update () {

		var r = arrow.GetComponent<MeshRenderer> ();
		r.enabled = !hidden;

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

			var rotH = Quaternion.AngleAxis(hAngle,Vector3.up);
			var vectorToAttentionPoint = rotH * Vector3.forward;
			var horAngleBeteenEyesAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x,vectorToAttentionPoint.z), new Vector2 (vectorToEyePosition.x,vectorToEyePosition.z));

			if (horAngleBeteenEyesAndTargetPoint > 0) {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (90, 0, -90));
			} else {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (-90, 0, -90));
			}
		}

		RotateArrowToFaceTarget();
	}

	private void RotateArrowToFaceTarget(){
		
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

	public void PointTowards(float hAng, float vAng) {
		hidden = false;
		following = false;
		hAngle = hAng;
		vAngle = vAng;
	}

	public void FollowTo(float hAng, float vAng) {
		hidden = false;
		following = true;
		hAngle = hAng;
		vAngle = vAng;
	}

	public void Clear(){
		hidden = true;
	}

}
