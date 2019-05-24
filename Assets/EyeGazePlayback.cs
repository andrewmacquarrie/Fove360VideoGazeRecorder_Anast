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
    public string videoPath;
    public GameObject mainCamera;
    public GameObject leftCursor;
    public GameObject rightCursor;
    public GameObject videoSphere;

    protected FileInfo theSourceFile = null;
    protected StreamReader reader = null;
    protected string text = " "; // assigned to allow first line to be read below

    private string[] lastData;

    // Use this for initialization
    void Start()
    {
        theSourceFile = new FileInfo(gazeFilePath);
        reader = theSourceFile.OpenText();
        text = reader.ReadLine(); // skip first line
        lastData = new string[] { "-1.0" };
    }

    // Update is called once per frame
    void Update()
    {
        var vp = videoSphere.GetComponent<VideoPlayer>();

        if (float.Parse(lastData[0]) < vp.time)
        {
            if (text != null)
            {
                text = reader.ReadLine();
                Debug.Log(text);
                var data = text.Split(","[0]);

                //ORDER IS: "video_time", "x_rot", "y_rot", "z_rot", "quat_x", "quat_y", "quat_z", "quat_w",
                // "left_eye_vector_x", "left_eye_vector_y", "left_eye_vector_z", "right_eye_vector_x", "right_eye_vector_y", "right_eye_vector_z",
                // "left_eye_texture_x", "left_eye_texture_y", "right_eye_texture_x", "right_eye_texture_y" };
                mainCamera.transform.rotation = new Quaternion(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]), float.Parse(data[7]));

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
        } else
        {
            Debug.Log("Not ready to playback yet");
        }

    }
}
