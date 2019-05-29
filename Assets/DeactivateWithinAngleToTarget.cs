using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateWithinAngleToTarget : MonoBehaviour {

	public float deactivationAngle;
	public GameObject objectToDeactivate;
	public GameObject eyeLocation;
	public GameObject target;

	private Vector2 targetSize;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var vectorToEyePosition = eyeLocation.transform.position;
		var vectorToTarget = target.transform.position;
		if(Vector3.Angle(vectorToEyePosition, vectorToTarget) < deactivationAngle){
			objectToDeactivate.SetActive(false);
		} else {
			objectToDeactivate.SetActive(true);
		}
	}

	public void UpdateTargetSize(AttentionEvent e){
		targetSize = new Vector2(e.width, e.height);
	}
}
