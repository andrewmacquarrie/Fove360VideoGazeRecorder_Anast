using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Video;
using System;

public class EyeGazePlayback : MonoBehaviour
{

    public string gazeFilePath;
    public string cueTypesFilePath;

    public string videoPath;
    public GameObject mainCamera;
    public GameObject leftCursor;
    public GameObject rightCursor;
    public GameObject videoSphere;

    public AttentionEventsController eventController;

    protected FileInfo theSourceFile = null;
    protected StreamReader reader = null;
    protected string text = " "; // assigned to allow first line to be read below

    private string[] lastData;

    private FoveEyeCamera leftEyeCamera;

    // Use this for initialization
    void Start()
    {
        var leftEyeCam = GameObject.Find("FOVE Eye (Left)");
        leftEyeCamera = leftEyeCam.GetComponent<FoveEyeCamera>();

        theSourceFile = new FileInfo(gazeFilePath);
        reader = theSourceFile.OpenText();
        text = reader.ReadLine(); // skip first line
        lastData = new string[] { "-1.0" };

        eventController.SetEventTypesForDebug(getCueTypes());
        //eventController.SetEventTypesForDebug(new List<string>(new string[] { "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW", "ARROW_FOLLOW" }));
    }

    private List<string> getCueTypes()
    {
        //Read the text from directly from the test.txt file
        StreamReader cueReader = new StreamReader(cueTypesFilePath);
        string cueCSV = cueReader.ReadToEnd();
        cueReader.Close();
        var lines = cueCSV.Split("\n"[0]);

        var cueT = new List<string>();
        
        for (int i = 1; i < lines.Length; i++) // skip header
        {
            var chunks = lines[i].Split(","[0]);
            if(chunks.Length > 1)
            {
                cueT.Add(chunks[1]);
            }
        }
        return cueT;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(leftEyeCamera.transform.position, leftEyeCamera.transform.forward, Color.red, 1f);

        var vp = videoSphere.GetComponent<VideoPlayer>();

        while (float.Parse(lastData[0]) < vp.time)
        {
            if (text != null)
            {
                text = reader.ReadLine();
                // Debug.Log(text);
                var data = text.Split(","[0]);

                //ORDER IS: "video_time", "x_rot", "y_rot", "z_rot", "quat_x", "quat_y", "quat_z", "quat_w",
                // "left_eye_vector_x", "left_eye_vector_y", "left_eye_vector_z", "right_eye_vector_x", "right_eye_vector_y", "right_eye_vector_z",
                // "left_eye_texture_x", "left_eye_texture_y", "right_eye_texture_x", "right_eye_texture_y" };
                var camDir = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));
                var eul = camDir.eulerAngles;
                eul = new Vector3(eul.x, eul.y - 90f, eul.z);

                mainCamera.transform.rotation = Quaternion.Euler(eul - leftEyeCamera.transform.forward);

                Ray leftEye = new Ray(new Vector3(0f, 0f, 0f), new Vector3(float.Parse(data[8]), float.Parse(data[9]), float.Parse(data[10])));

                RaycastHit hit;
                MeshCollider coll = videoSphere.GetComponent<MeshCollider>();
                if (coll != null && coll.Raycast(leftEye, out hit, Mathf.Infinity))
                {
                    leftCursor.transform.position = hit.point;
                }
                Ray rightEye = new Ray(new Vector3(0f, 0f, 0f), new Vector3(float.Parse(data[11]), float.Parse(data[12]), float.Parse(data[13])));
                if (coll != null && coll.Raycast(rightEye, out hit, Mathf.Infinity))
                {
                    rightCursor.transform.position = hit.point;
                }

                lastData = data;
            }
        } 

    }
}
