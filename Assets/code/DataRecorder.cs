using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using UnityEngine.Video;
using System;


public class DataRecorder : MonoBehaviour {
    public int participantId;

	private string orientationDataFilePath;

    public GameObject fove;
    public GameObject videoSphere;
    private FoveInterface foveInterface;

    private float nextRecord = 0.0F; 
	private string delimiter = ",";
    private float recordRate = 0.02f; // 50 frames per second  
    //private float recordRate = 0.1f; // 10 frames per second  
    //private float recordRate = 0.1f; // 10 frames per second  

    private TextWriter orientationStringWriter;

    // Use this for initialization
    void Start ()
    {
        Scene scene = SceneManager.GetActiveScene();
        orientationDataFilePath = "./DataRecordings/" + participantId + "_" + scene.name + DateTime.Now.ToString("MM-dd-yy_hh-mm-ss") + ".csv";
        
        orientationStringWriter = new StreamWriter(orientationDataFilePath);
        string[] headers = new string[] { "video_time", "x_rot", "y_rot", "z_rot", "quat_x", "quat_y", "quat_z", "quat_w",
            "left_eye_vector_x", "left_eye_vector_y", "left_eye_vector_z", "right_eye_vector_x", "right_eye_vector_y", "right_eye_vector_z",
            "left_eye_texture_x", "left_eye_texture_y", "right_eye_texture_x", "right_eye_texture_y" };
        orientationStringWriter.Write(string.Join(delimiter, headers) + "\n");

        foveInterface = fove.GetComponent<FoveInterface>();
    }

	void Update() {
        var vp = videoSphere.GetComponent<VideoPlayer>();


        if (vp.isPlaying) {
			nextRecord = Time.time + recordRate;
            
            var quaternion = FoveInterface.GetHMDRotation();
            var euler       = quaternion.eulerAngles;

            //var leftEyeVector = FoveInterface.GetLeftEyeVector();
            //var rightEyeVector = FoveInterface.GetRightEyeVector();

            //var leftRay = new Ray(foveInterface.GetEyeCamera(Fove.EFVR_Eye.Left).transform.position, leftEyeVector);
            //var rightRay = new Ray(foveInterface.GetEyeCamera(Fove.EFVR_Eye.Right).transform.position, rightEyeVector);
            
            FoveInterface.EyeRays rays = FoveInterface.GetEyeRays();
            var leftRay = rays.left;
            var rightRay = rays.right;


            //Debug.DrawRay(leftRay.origin, leftRay.direction, Color.green, 2, false);
            //Debug.DrawRay(rightRay.origin, rightRay.direction, Color.blue, 2, false);

            Vector2 pixeluvLeft = GetPixelTextureCoords(leftRay);
            Vector2 pixeluvRight = GetPixelTextureCoords(rightRay);

            string[] data = new string[]{ vp.time.ToString(), euler.x.ToString(), euler.y.ToString(), euler.z.ToString(), quaternion.x.ToString(), quaternion.y.ToString(), quaternion.z.ToString(), quaternion.w.ToString(),
                leftRay.direction.x.ToString(), leftRay.direction.y.ToString(), leftRay.direction.z.ToString(), rightRay.direction.x.ToString(), rightRay.direction.y.ToString(), rightRay.direction.z.ToString(),
                pixeluvLeft.x.ToString(), pixeluvLeft.y.ToString(), pixeluvRight.x.ToString(), pixeluvRight.y.ToString()};

            orientationStringWriter.Write(string.Join(delimiter, data) + "\n");
        }
    }

    private Vector2 GetPixelTextureCoords(Ray r)
    {
        RaycastHit hit;
        MeshCollider coll = videoSphere.GetComponent<MeshCollider>();
        if (coll != null && coll.Raycast(r, out hit, Mathf.Infinity))
        {
            //Texture2D tex = videoSphere.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
            //Texture2D tex = videoSphere.GetComponent<VideoPlayer>().targetTexture.material.mainTexture as Texture2D;

            float width = videoSphere.GetComponent<VideoPlayer>().targetTexture.width;
            float height = videoSphere.GetComponent<VideoPlayer>().targetTexture.height;

            Vector2 pixelUV = hit.textureCoord;
            float pixelX = pixelUV.x * width;
            float pixelY = pixelUV.y * height;

            //Debug.Log("Texture A size: " + width + ", " + height);
            Debug.Log("Centre position in pano A: " + pixelX + ", " + pixelY);

            return new Vector2(pixelX, pixelY);
        }

        return new Vector2(0f,0f);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds. Closing file writer from Data Recorder");
        orientationStringWriter.Close();
    }
}
