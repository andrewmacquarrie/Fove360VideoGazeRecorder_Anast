using UnityEngine;
using System.Collections;

public class FOVE3DCursorRight : MonoBehaviour {

    public GameObject videoSphere;
	
	// Use this for initialization
	void Start () {
	}

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update() {
        FoveInterface.EyeRays rays = FoveInterface.GetEyeRays();
        Ray r = rays.right;

        RaycastHit hit;
        MeshCollider coll = videoSphere.GetComponent<MeshCollider>();
        if (coll != null && coll.Raycast(r, out hit, Mathf.Infinity))
        {
            transform.position = hit.point;
        }   
	}
}
