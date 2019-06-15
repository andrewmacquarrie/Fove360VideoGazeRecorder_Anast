using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndOnVideoFinished : MonoBehaviour {



	// Use this for initialization
	void Start () {
        VideoPlayer vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += EndReached;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        Application.Quit();
    }
}
