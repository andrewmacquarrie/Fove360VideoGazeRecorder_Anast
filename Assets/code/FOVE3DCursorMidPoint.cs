using System;
using UnityEngine;
using System.Collections;

public class FOVE3DCursorMidPoint : MonoBehaviour {

    public GameObject videoSphere;
	
	// Use this for initialization
	void Start () {
	}

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update() {
        Ray r ;
        Ray l;
        try {
            FoveInterface.EyeRays rays = FoveInterface.GetEyeRays();
            r = rays.right;
            l = rays.left;
        } catch (Exception e) {
            Debug.Log("Failed to get eye rays - probably FOVE not attached");
            r = new Ray(new Vector3(0,0,0), Vector3.forward);
            l = new Ray(new Vector3(0,0,0), Vector3.forward);
        }

        var mutualDirection = r.direction + l.direction;
        var mutualPosition = Vector3.Lerp(r.origin, l.origin, 0.5f);
        var mutualRay = new Ray(mutualPosition,mutualDirection);

        RaycastHit hit;
        MeshCollider coll = videoSphere.GetComponent<MeshCollider>();
        if (coll != null && coll.Raycast(mutualRay, out hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }   
	}
}
