using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTarget : MonoBehaviour {

	public GameObject eyeLocation;
	private float hAngle;
	private float vAngle;
	public GameObject arrow;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RotateToTarget();
	}

	public void SetTarget(float hAng, float vAng){
		hAngle = hAng;
		vAngle = vAng;
	}

	private void RotateToTarget(){
		var vectorToEyePosition = eyeLocation.transform.position;

		var horAngleBetweenArrowAndEyes = Vector2.SignedAngle (new Vector2 (transform.position.x,transform.position.z), new Vector2 (vectorToEyePosition.x,vectorToEyePosition.z));
		
		var rotH = Quaternion.AngleAxis(hAngle,Vector3.up);
		var vectorToAttentionPoint = rotH * new Vector3(-1,0,0); // Vector3.forward;
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
			var longLatsForEyePosition = AngleHelperMethods.PositionToLonLat(vectorToEyePosition); //   GetLatLongForEyePosition(vectorToEyePosition);
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

	private Vector2 GetLatLongForEyePosition(Vector3 vectorToEyePosition)
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
}
