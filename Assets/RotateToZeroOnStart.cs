using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RotateToZeroOnStart : MonoBehaviour {
    private GameObject mainCamera;
    private Vector3 forwardAtStart;

    public VideoPlayer vp;
    
	// Use this for initialization
	void Start () {
        forwardAtStart = transform.forward;

        mainCamera = GameObject.Find("FOVE Eye (Left)");

        if (mainCamera == null)
        { // the FOVE isn't connected - just use the regular camera
            mainCamera = GameObject.Find("Fove Interface");
        }

        RotateToFaceForward();
    }

    private bool done = false;
    
    // Update is called once per frame
    void Update()
    {
        if(!done && Time.frameCount > 2f)
        {
            done = true;
            RotateToFaceForward();
        }
    }
    
    void RotateToFaceForward()
    {
        FoveEyeCamera ec = mainCamera.GetComponent<FoveEyeCamera>();
        var eul = new Vector3(0f, 180f, 0f);
        transform.rotation = Quaternion.Euler(0f, eul.y - ec.transform.rotation.eulerAngles.y, 0f);
    }
}
