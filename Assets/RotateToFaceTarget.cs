using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTarget : MonoBehaviour {
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
		var vectorToArrowPosition = arrow.transform.position;

		var rotH = Quaternion.AngleAxis(hAngle,Vector3.up);
		var vectorToAttentionPoint = rotH * new Vector3(-1,0,0); // Vector3.forward;
		var horAngleBeteenArrowAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x,vectorToAttentionPoint.z), new Vector2 (vectorToArrowPosition.x,vectorToArrowPosition.z));

		if(horAngleBeteenArrowAndTargetPoint > 90 || horAngleBeteenArrowAndTargetPoint < -90) {
			// the target is on the other side - need to point left/right to assure arrow does't point over the top/bottom of sphere based on nearest bearing
			if (horAngleBeteenArrowAndTargetPoint > 0) {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (90, 0, -90));
			} else {
				arrow.transform.localRotation = Quaternion.Euler(new Vector3 (-90, 0, -90));
			} 
		} else {
			// target is on this side of the sphere - make the arrow point to the correct bearing
			var longLatsForArrowPosition = AngleHelperMethods.PositionToLonLat(vectorToArrowPosition); //   GetLatLongForEyePosition(vectorToEyePosition);
			//Debug.LogError(longLatsForEyePosition);
			var longLatsForAttentionPosition = new Vector2(hAngle, vAngle);
			var bearingToTarget = GetBearingForLongLats(horAngleBeteenArrowAndTargetPoint, longLatsForArrowPosition, longLatsForAttentionPosition);

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
}
