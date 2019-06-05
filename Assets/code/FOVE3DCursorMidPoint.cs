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
        /* 
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
        }*/

        try {
            UpdatePositionBasedOnEyes();  
        } catch (Exception e) {
            // Debug.LogError("Failed to get eye rays - probably FOVE not attached");
        }
	}

    // Update is called once per frame
	private void UpdatePositionBasedOnEyes () {
        // this is from here.. maybe better robustness against lost tracking: https://github.com/twday/Fove-Unity-Examples/blob/master/Assets/Examples/FoveCursor/Scripts/FoveCursor.cs

        FoveInterface.EyeRays eyes = FoveInterface.GetEyeRays();
        RaycastHit hitLeft, hitRight;

        switch (FoveInterface.CheckEyesClosed())
        {
            case Fove.EFVR_Eye.Neither:

                Physics.Raycast(eyes.left, out hitLeft, Mathf.Infinity);
                Physics.Raycast(eyes.right, out hitRight, Mathf.Infinity);
                if (hitLeft.point != Vector3.zero && hitRight.point != Vector3.zero)
                {
                    transform.position = hitLeft.point + ((hitRight.point - hitLeft.point) / 2);
                } else
                {
                    transform.position = eyes.left.GetPoint(3.0f) + ((eyes.right.GetPoint(3.0f) - eyes.left.GetPoint(3.0f)) / 2);
                }

                break;
            case Fove.EFVR_Eye.Left:

                Physics.Raycast(eyes.right, out hitRight, Mathf.Infinity);
                if (hitRight.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
                {
                    transform.position = hitRight.point;
                }
                else
                {
                    transform.position = eyes.right.GetPoint(3.0f);
                }
                break;
            case Fove.EFVR_Eye.Right:  

                Physics.Raycast(eyes.left, out hitLeft, Mathf.Infinity);
                if (hitLeft.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
                {
                    transform.position = hitLeft.point;
                }
                else
                {
                    transform.position = eyes.left.GetPoint(3.0f);
                }
                break;
        }
	}
}
