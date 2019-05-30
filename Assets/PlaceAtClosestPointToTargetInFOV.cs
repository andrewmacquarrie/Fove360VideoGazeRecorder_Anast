using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAtClosestPointToTargetInFOV : MonoBehaviour {
	// public GameObject eyeLocation;
	public GameObject target;
	public GameObject camera;
	public float m_edgeBuffer = 30f;

	private Camera mainCamera;
	private float distanceToFlicker;

	// Use this for initialization
	void Start () {
		mainCamera = camera.GetComponent<Camera>();
		distanceToFlicker = transform.position.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePositionWithinFov();

		/* 
		var vectorToTarget = target.transform.position;
		var vectorToHeadPosition = camera.transform.forward;
		var horAngleBeteenHeadAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToTarget.x,vectorToTarget.z), new Vector2 (vectorToHeadPosition.x,vectorToHeadPosition.z));

		if(horAngleBeteenHeadAndTargetPoint > 90 || horAngleBeteenHeadAndTargetPoint < -90) {
			// the target is on the other side - need to point left/right to assure arrow does't point over the top/bottom of sphere based on nearest bearing
			if (horAngleBeteenEyesAndTargetPoint > 0) {
				// set to right FOV
				
				
				// arrow.transform.localRotation = Quaternion.Euler(new Vector3 (90, 0, -90));
			} else {
				// set to left FOV

				//arrow.transform.localRotation = Quaternion.Euler(new Vector3 (-90, 0, -90));
			} 
		} else {
			// find 3D angle to target from HEAD direction

			// if is greater than FOV / 2, set to FOV / 2

			// if it's less than FOV, place on target

		}*/
	}


	// From https://github.com/Mouledoux/ConnectedHome/blob/master/Assets/Scripts/TargetIndicator.cs
	private void UpdatePositionWithinFov()
    {		
		var vectorToAttentionPoint = target.transform.position;
		var vectorToHeadPosition = camera.transform.forward;
		var horAngleBeteenHeadAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x,vectorToAttentionPoint.z), new Vector2 (vectorToHeadPosition.x,vectorToHeadPosition.z));

		var bearingToTarget = GetBearingForLongLats(horAngleBeteenHeadAndTargetPoint, AngleHelperMethods.PositionToLonLat(camera.transform.forward), AngleHelperMethods.PositionToLonLat(vectorToAttentionPoint));
		Debug.LogError("B: " + bearingToTarget);

        Vector3 newPos = target.transform.position;

        newPos = mainCamera.WorldToViewportPoint(newPos);

        if(newPos.z < 0)
        {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;

            newPos = Vector3Maxamize(newPos);
        }

        newPos = mainCamera.ViewportToScreenPoint(newPos);

        newPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
        newPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);

		Ray rayToSceneFlicker = mainCamera.ScreenPointToRay(newPos);

		Debug.DrawRay(rayToSceneFlicker.origin, rayToSceneFlicker.direction, Color.green, 1f);

		SetAnglesForFlicker(rayToSceneFlicker);

        //m_icon.transform.position = newPos;
    }

    public Vector3 Vector3Maxamize(Vector3 vector)
    {
        Vector3 returnVector = vector;

        float max = Mathf.Max(vector.x, vector.y, 0f);
        if (max != 0f) {
            returnVector /= max;
        }

        return returnVector;
    }

	private void SetAnglesForFlicker(Ray rayToSceneFlicker){
		// var rotation = Quaternion.FromToRotation(camera.transform.forward, rayToSceneFlicker.direction);
		var oldLatLong = AngleHelperMethods.PositionToLonLat(transform.position);
		var newLatLong = AngleHelperMethods.PositionToLonLat(rayToSceneFlicker.direction);
		
		var longChange = oldLatLong.x - newLatLong.x;
		var latChange = oldLatLong.y - newLatLong.y;

		// Debug.LogError("long change: " + longChange);
		// Debug.LogError("lat change: " + latChange);

		var up = new Vector3(0,1,0);
		transform.RotateAround(Vector3.zero, up, longChange * -1f);
		transform.RotateAround(Vector3.zero, transform.forward, latChange);
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
