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
        try {
            FoveInterface.EyeRays rays = FoveInterface.GetEyeRays();
            Ray r = rays.right;
            Ray l = rays.left;

            var mutualDirection = r.direction + l.direction;
            var mutualPosition = Vector3.Lerp(r.origin, l.origin, 0.5f);
            var mutualRay = new Ray(mutualPosition,mutualDirection);

            RaycastHit hit;
            MeshCollider coll = videoSphere.GetComponent<MeshCollider>();
            if (coll != null && coll.Raycast(mutualRay, out hit, Mathf.Infinity))
            {
                transform.position = hit.point;
            }   
        } catch (Exception e) {
            // Debug.LogError("Failed to get eye rays - probably FOVE not attached");
        }
	}
}
