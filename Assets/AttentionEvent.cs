using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class AttentionEvent
{
    public float startTime;
    public float width;
    public float height;
    public string type;
    public float targetHorPixel;
    public float targetVerPixel;

    public float hAngle {
        get {
            return AngleHelperMethods.PixelCoordToLong(targetHorPixel);
        }
    }
    public float vAngle {
        get {
            return AngleHelperMethods.PixelCoordToLat(targetVerPixel);
        }
    }

}