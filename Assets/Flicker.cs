using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flicker : MonoBehaviour {

	Image img;

	bool blueHigh;

	public float bluePercentageChange = 0.12f;

	public float secondsBetweenFlickers = 0.1f;

	// Use this for initialization
	void Start () {
		img = GetComponent<Image> ();
		InvokeRepeating("Toggle", 0.0f, secondsBetweenFlickers);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Toggle(){
		blueHigh = !blueHigh;
		if (blueHigh) {
			img.color = new Color (0.51f, 0.51f, 0.51f * (1f + bluePercentageChange), 0.35f);
		} else {
			img.color = new Color (0.51f, 0.51f, 0.51f * (1f - bluePercentageChange), 0.35f);
		}
	}
}
