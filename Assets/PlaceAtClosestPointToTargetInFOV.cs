using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAtClosestPointToTargetInFOV : MonoBehaviour {
	// public GameObject eyeLocation;
	public GameObject target;
	// public GameObject camera;

	private GameObject leftEyeCamera;
	private GameObject rightEyeCamera;
	public float m_edgeBuffer = 30f;

	//private Camera mainCamera;
	private float distanceToFlicker;
	public GameObject tangentRotate;
	public GameObject tangentPoint;

	// Use this for initialization
	void Start () {
		//mainCamera = camera.GetComponent<Camera>();
		distanceToFlicker = transform.position.magnitude;

		leftEyeCamera = GameObject.Find("FOVE Eye (Left)");
		rightEyeCamera = GameObject.Find("FOVE Eye (Right)");

		if(leftEyeCamera == null){ // the FOVE isn't connected - just use the regular camera
			leftEyeCamera = GameObject.Find("Fove Interface");
			rightEyeCamera = GameObject.Find("Fove Interface");
		}
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
		var newPos = GetScreenPointOfAttentionTarget(target);
		var leftEyeCam = leftEyeCamera.GetComponent<Camera>();

		var clampedPos = new Vector2();
        clampedPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, leftEyeCam.scaledPixelWidth - m_edgeBuffer); // (val, min, max)
        clampedPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, leftEyeCam.scaledPixelHeight - m_edgeBuffer);

		if(clampedPos.x != newPos.x || clampedPos.y != newPos.y){
			// the target doesn't fall inside the viewport, so we should place flicker on bearing to target on sphere
			var vectorToAttentionPoint = target.transform.position;
			var vectorToHeadPosition = leftEyeCamera.transform.forward;

			var horAngleBeteenHeadAndTargetPoint = Vector2.SignedAngle (new Vector2 (vectorToAttentionPoint.x,vectorToAttentionPoint.z), new Vector2 (vectorToHeadPosition.x,vectorToHeadPosition.z));
			
			//Debug.LogError(horAngleBeteenHeadAndTargetPoint);

			if(horAngleBeteenHeadAndTargetPoint > 90 || horAngleBeteenHeadAndTargetPoint < -90) {
				// the target is on the other side - need to point left/right to assure arrow does't point over the top/bottom of sphere based on nearest bearing
				if (horAngleBeteenHeadAndTargetPoint > 0) {
					tangentRotate.transform.localRotation = Quaternion.Euler(new Vector3 (0,90f,0f));
				} else {
					tangentRotate.transform.localRotation = Quaternion.Euler(new Vector3 (180f,90f,0f));
				} 
			} else {
				var longLatForHead = AngleHelperMethods.PositionToLonLat(leftEyeCamera.transform.forward);
				var longLatForTarget = AngleHelperMethods.PositionToLonLat(vectorToAttentionPoint);

				var bearingToTarget = GetBearingForLongLats(horAngleBeteenHeadAndTargetPoint, longLatForHead, longLatForTarget);

				// DEBUG: THIS DOESNT SEEM TO WORK QUITE RIGHT - but is it enough for a pilot study?	
				var rollOfCamera = leftEyeCamera.transform.rotation.eulerAngles.z;		
				tangentRotate.transform.localRotation = Quaternion.Euler(bearingToTarget - 90f + rollOfCamera,90f,0f);
			}
			
			newPos = GetScreenPointOfAttentionTarget(tangentPoint);

			clampedPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, leftEyeCam.scaledPixelWidth - m_edgeBuffer); // (val, min, max)
        	clampedPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, leftEyeCam.scaledPixelHeight - m_edgeBuffer);

		}

		Ray rayToSceneFlicker = leftEyeCam.ScreenPointToRay(clampedPos);
		Debug.DrawRay(rayToSceneFlicker.origin, rayToSceneFlicker.direction, Color.green, 1f);
		SetAnglesForFlicker(rayToSceneFlicker);

        //m_icon.transform.position = newPos;
    }

	private Vector2 GetScreenPointOfAttentionTarget(GameObject attentionTarget){
		var vectorToAttentionPoint = attentionTarget.transform.position;
		var vectorToHeadPosition = leftEyeCamera.transform.forward;
		//Debug.LogError("B: " + bearingToTarget);

		var leftEyeCam = leftEyeCamera.GetComponent<Camera>();

        Vector3 newPos = attentionTarget.transform.position;

        newPos = leftEyeCam.WorldToViewportPoint(newPos);


        if(newPos.z < 0)
        {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;

            newPos = Vector3Maxamize(newPos);
        }

        newPos = leftEyeCam.ViewportToScreenPoint(newPos);

		Debug.Log("viewpoint pixel position: " + newPos);

		return newPos;
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
