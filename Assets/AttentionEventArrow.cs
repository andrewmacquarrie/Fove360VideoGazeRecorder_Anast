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

			RotateArrowToFaceTarget();
		}

	}

	private void RotateArrowToFaceTarget(){
		var vectorToEyePosition = eyeLocation.transform.position;

		var rotH = Quaternion.AngleAxis(hAngle,Vector3.up);
		var vectorToAttentionPoint = rotH * Vector3.forward;
		var horAngleBeteenEyesAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x,vectorToAttentionPoint.z), new Vector2 (vectorToEyePosition.x,vectorToEyePosition.z));

		if(horAngleBeteenEyesAndTargetPoint > 90 || horAngleBeteenEyesAndTargetPoint < -90) {
			// the target is on the other side - need to point left/right to assure arrow does't point over the top/bottom of sphere based on nearest bearing
			if (horAngleBeteenEyesAndTargetPoint > 0) {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (90, 0, -90));
			} else {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (-90, 0, -90));
			} 
		} else {
			// target is on this side of the sphere - make the arrow point to the correct bearing
			var longLatsForEyePosition = GetLatLongForEyePosition(horAngleBeteenEyesAndTargetPoint, vectorToEyePosition);
			//Debug.LogError(longLatsForEyePosition);
			var longLatsForAttentionPosition = new Vector2(hAngle, vAngle);
			var bearingToTarget = GetBearingForLongLats(horAngleBeteenEyesAndTargetPoint, longLatsForEyePosition, longLatsForAttentionPosition);

			arrow.transform.localRotation = Quaternion.Euler(new Vector3 (bearingToTarget, 0, -90));
		}
	}

	private float GetBearingForLongLats(float horAngleBeteenEyesAndTargetPoint, Vector2 longLatsForEyePosition, Vector2 longLatsForAttentionPosition){
		var dLon = horAngleBeteenEyesAndTargetPoint * (1f / Mathf.Rad2Deg);
		var lat2 = longLatsForAttentionPosition.y * (1f / Mathf.Rad2Deg);
		var lat1 = longLatsForEyePosition.y * (1f / Mathf.Rad2Deg);

		var y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
		var x = Mathf.Cos(lat1) * Mathf.Sin(lat2) - Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon);
		var brng = Mathf.Atan2(y, x);

		return brng * Mathf.Rad2Deg;
	}

	private Vector2 GetLatLongForEyePosition(float horAngleBeteenEyesAndTargetPoint, Vector3 vectorToEyePosition)
	{
		var latLongDeg = ToLatLongRad(vectorToEyePosition) * Mathf.Rad2Deg;

		if( float.IsNaN(latLongDeg.x) ){
			latLongDeg.x = 0f;
		}
		if( float.IsNaN(latLongDeg.y) ){
			latLongDeg.y = 0f;
		}

		return latLongDeg;
	}

	public Vector2 ToLatLongRad(Vector3 position)
	{
		position.Normalize();

		var lng = -( Mathf.Atan2( -position.z, -position.x ) ) - Mathf.PI / 2f;

		//to bind between -PI / PI
		if( lng < - Mathf.PI ){
			lng += Mathf.PI * 2;
		}

		//latitude : angle between the vector & the vector projected on the XZ plane on a unit sphere

		//project on the XZ plane
		var p = new Vector3( position.x, 0f, position.z );
		//project on the unit sphere
		p.Normalize();

		//commpute the angle ( both vectors are normalized, no division by the sum of lengths )
		var lat = Mathf.Acos( Vector3.Dot(p, position) );

		//invert if Y is negative to ensure teh latitude is comprised between -PI/2 & PI / 2
		if( position.y < 0 ){
			lat *= -1;
		}

		return new Vector2(lng,lat);
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
