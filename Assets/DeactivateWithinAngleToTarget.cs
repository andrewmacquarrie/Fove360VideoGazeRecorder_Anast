using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateWithinAngleToTarget : MonoBehaviour {

	public float deactivationAngle;
	public float reactivationAngle;
	public AttentionEventArrow objectToDeactivateArrow;
	public FlickerDotController objectToDeactivateFlicker;
	public GameObject eyeLocation;
	public GameObject target;

	public GameObject deactivateBasedOnThisObjectsLocation;

	private Vector2 targetSize;
	private AttentionEvent e;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(e == null){
			// no targets happened yet
			return;
		}

		// get the pixel coords for the target and eye positions
		var targetPixelCoords = AngleHelperMethods.PositionToPixelCoords(target.transform.position);
		var eyePixelCoords = AngleHelperMethods.PositionToPixelCoords(eyeLocation.transform.position);

		// calculate the pixel coord for the closest point in the target to the eye position
		var closestPointOnTarget = AngleHelperMethods.GetClosestPointOnTarget(e, eyePixelCoords);

		// calculate the long/lat for that pixel coord
		var closestPointOnTargetLonLat = new Vector2(AngleHelperMethods.PixelCoordToLong(closestPointOnTarget.x), AngleHelperMethods.PixelCoordToLat(closestPointOnTarget.y));

		// get long/lat as rays?
		var vectorToClosestPointOnTarget = AngleHelperMethods.LonLatToPosition(closestPointOnTargetLonLat.x, closestPointOnTargetLonLat.y);

		// show this on the screen as a ray
		//Debug.DrawRay(new Vector3(0,0,0), vectorToClosestPointOnTarget, Color.green, 1f);

		// claculate 3d angle between these, and add deactivationAngle to that - if less than this, deactivate object
		var vectorToTarget = target.transform.position;
		var angleTargetCentreToClosestTargetPoint = Vector3.Angle(vectorToTarget, vectorToClosestPointOnTarget);
		var totalAllowableAngleForDeactivation = angleTargetCentreToClosestTargetPoint + deactivationAngle;
		var totalAllowableAngleForReactivation = angleTargetCentreToClosestTargetPoint + reactivationAngle;
		var vectorToObjectDeactivationIsBasedOn = deactivateBasedOnThisObjectsLocation.transform.position;

		if(Vector3.Angle(vectorToObjectDeactivationIsBasedOn, vectorToTarget) < totalAllowableAngleForDeactivation){
			// deactivation angle reached
			if(objectToDeactivateArrow != null) {
				objectToDeactivateArrow.ActivationStatusChangedByAngleToTarget(false);
			} else {
				objectToDeactivateFlicker.ActivationStatusChangedByAngleToTarget(false);
			}
		} else if (Vector3.Angle(vectorToObjectDeactivationIsBasedOn, vectorToTarget) > totalAllowableAngleForReactivation) {
			if(objectToDeactivateArrow != null) {
				objectToDeactivateArrow.ActivationStatusChangedByAngleToTarget(true);
			} else {
				objectToDeactivateFlicker.ActivationStatusChangedByAngleToTarget(true);
			}
		}
	}

	public void UpdateTargetSize(AttentionEvent ev){
		targetSize = new Vector2(ev.width, ev.height);
		e = ev;
	}
}
