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
    
    public TextAsset eventsFile;

    private List<AttentionEvent> events;
	int currentEventIndex;

	private string currentCueTypeUntilCleared = "";

    private List<string> attentionEventTypes;

	[System.Serializable]
	public class AllAttentionEventData
	{
		public List<AttentionEvent> events = new List<AttentionEvent>();
	}

	private List<AttentionEvent> LoadAttentionEvents(){
        string dataAsJson = eventsFile.text;
        var allEventsContainer = JsonUtility.FromJson<AllAttentionEventData> (dataAsJson);
        return allEventsContainer.events;
	}

    private void RandomiseListOrder(List<string> eventTypes)
    {
        for (int i = 0; i < eventTypes.Count; i++)
        {
            string temp = eventTypes[i];
            int randomIndex = Random.Range(i, eventTypes.Count);
            eventTypes[i] = eventTypes[randomIndex];
            eventTypes[randomIndex] = temp;
        }
    }

	// Use this for initialization
	void Start () {
        attentionEventTypes = new List<string> { "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW", "FLICKER", "FLICKER", "FLICKER", "FLICKER" };
        RandomiseListOrder(attentionEventTypes);
        
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
                    /*
					var randSelector =  Random.value;
					// randSelector = 0.5f; // allows debug of one type of cue
					if(randSelector > 0.5f) {
						currentCueTypeUntilCleared = "ARROW_FOLLOW";
					} else {
						currentCueTypeUntilCleared = "FLICKER";
					}*/

                    currentCueTypeUntilCleared = attentionEventTypes[0];
                    attentionEventTypes.RemoveAt(0);
                    
                    if (dataRecorder.activeSelf) {
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
