using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AngleHelperMethods  {
    private static float horizontalResolution = 3840f;
    private static float verticalResolution = 1920f;
    
    public static float PixelCoordToLong(float pixelCoord){
        var percentageAcross = pixelCoord / horizontalResolution;
        var angle = percentageAcross * 360f;
        return angle - 180f; // need to bring centre to zero
    }

    public static float PixelCoordToLat(float pixelCoord){
        var percentageDown = pixelCoord / verticalResolution;
        var angle = percentageDown * 180f;
        return (angle - 90f) * -1f; // need to bring centre to zero, plus also reverse as measured from top left whereas minus should be below horizon
    }
}
