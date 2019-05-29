using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyLongLat : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ApplyLongLatToPosition(float longitude, float latitude){
		transform.position = AngleHelperMethods.LonLatToPosition(longitude, latitude);
	}
}
