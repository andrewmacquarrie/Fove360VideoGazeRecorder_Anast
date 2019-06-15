using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToZeroOnStart : MonoBehaviour {


    private GameObject mainCamera;
    
	// Use this for initialization
	void Start () {

        //mainCamera = GameObject.Find("FOVE Eye (Left)");

        if (mainCamera == null)
        { // the FOVE isn't connected - just use the regular camera
            mainCamera = GameObject.Find("Fove Interface");
        }
        
//        Debug.LogError(mainCamera.transform.rotation.eulerAngles.y);
        transform.rotation = Quaternion.Euler(0f, 200f - mainCamera.transform.localRotation.eulerAngles.y, 0f);
 //       Debug.LogError(mainCamera.transform.rotation.eulerAngles.y);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
