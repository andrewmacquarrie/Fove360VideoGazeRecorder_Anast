using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

public class AttentionEventsController : MonoBehaviour {

	public AttentionEventArrow arrow;
	public FlickerDotController flickerController;
	public DrawTargetRect drawTarget;

	public GameObject dataRecorder;

	public VideoPlayer videoPlayer;

	public ApplyLongLat target;

	public string PathToEventsFile;
	private List<AttentionEvent> events;
	int currentEventIndex;

	private string currentCueTypeUntilCleared = "";

	[System.Serializable]
	public class AllAttentionEventData
	{
		public List<AttentionEvent> events = new List<AttentionEvent>();
	}

	private List<AttentionEvent> LoadAttentionEvents(){
		if (File.Exists (PathToEventsFile)) {
			string dataAsJson = File.ReadAllText (PathToEventsFile);
			var allEventsContainer = JsonUtility.FromJson<AllAttentionEventData> (dataAsJson);
			return allEventsContainer.events;
		}
		Debug.Log ("Not able to load events - file path doesnt contain any events: " + PathToEventsFile);
		return new List<AttentionEvent> ();
	}

	// Use this for initialization
	void Start () {
		events = LoadAttentionEvents ();

		foreach (var e in events) {
			Debug.Log (e.startTime);
		}

		currentEventIndex = 0;

		//Debug.LogError(AngleHelperMethods.GetClosestPointOnTarget(new AttentionEvent() { targetHorPixel = 2000, targetVerPixel = 960, height = 10, width = 10 }, new Vector2(1920,500)));
	}
	
	// Update is called once per frame
	void Update () {
		if (currentEventIndex >= events.Count) {
			return;
		}

		var currentEvent = events [currentEventIndex];

		if (videoPlayer.time > currentEvent.startTime) {
			if (currentEvent.type == "CLEAR") { // first, see if the event type from the file is telling us to stop the cue!
				arrow.Clear ();
				flickerController.Clear();
				currentCueTypeUntilCleared = "";
			} 
			else { // otherwise, use the randomly generated event type
				if(currentCueTypeUntilCleared == "") {
					if(Random.value > 0.5f) {
						currentCueTypeUntilCleared = "ARROW_FOLLOW";
					} else {
						currentCueTypeUntilCleared = "FLICKER";
					}
					if(dataRecorder.activeSelf) {
						DataRecorder dr = dataRecorder.GetComponent<DataRecorder>();
						dr.RecordCueType(currentCueTypeUntilCleared);
					}
				}

				target.ApplyLongLatToPosition(currentEvent.hAngle, currentEvent.vAngle);
				if (currentCueTypeUntilCleared == "ARROW_FOLLOW") { 
					arrow.FollowTo (currentEvent);
				} else if (currentCueTypeUntilCleared == "FLICKER") {
					flickerController.PointTowards (currentEvent);
				}

				// only for debug!
				drawTarget.SetTargetBox(currentEvent);
			}	
			currentEventIndex++;
		}
	}
}
