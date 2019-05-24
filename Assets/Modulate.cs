using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modulate : MonoBehaviour {

	public float frequency = 10f; // frequency in hz
	public Material whiteMat;
	public Material blackMat;
	public float intensity = 0.3f;

	private bool modulating;
	//private float t;

	private float timeToT;
	private float currentOscillationTime;
	private bool goingBlack;

	// Use this for initialization
	void Start () {
		modulating = true;
		timeToT = 1f / frequency;
	}
	
	// Update is called once per frame
	void Update () {
		currentOscillationTime += Time.deltaTime;
		if (currentOscillationTime > timeToT) { // we've reached the end, turn back
			currentOscillationTime = Mathf.Repeat(currentOscillationTime, timeToT); // modulus
			goingBlack = !goingBlack;
		}

		var t = currentOscillationTime / timeToT; // take into range 0-1

		if (goingBlack) {
			whiteMat.color = new Color (1f, 1f, 1f, (1f - t) * intensity);
			blackMat.color = new Color (1f, 1f, 1f, t * intensity);
		} else {
			whiteMat.color = new Color (1f, 1f, 1f, t * intensity);
			blackMat.color = new Color (1f, 1f, 1f, (1f - t) * intensity);
		}

		/*
		var white = whiteMat.color;
		white.a = t; //(t * 255f);

		Debug.Log ("white " + white.a );

		var black = blackMat.color;
		black.a = 1f-t; // (1f-t) * 255f;

		Debug.Log ("black" + black.a );*/
	}
}
