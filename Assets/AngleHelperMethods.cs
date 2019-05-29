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

    public static float LongToPixelCoordX(float lon){
        var percentageAround = lon / 180f;
        var pixelX = (percentageAround * (horizontalResolution/2f)) + (horizontalResolution / 2f); // as 0 deg long is centered at pixel coords (horizontalResolution/2f)
        pixelX = Mathf.Repeat(pixelX, horizontalResolution) + 1f; // make sure is in 360 range, ie from 1 to horizontalResolution
        return pixelX;
    }

    public static float LatToPixelCoordY(float lat){
        var percentageDown = lat / 90f;
        var pixelY = (percentageDown * (verticalResolution/2f)) + (verticalResolution / 2f); // as 0 deg long is centered at pixel coords (horizontalResolution/2f)
        // shouldny need this - what is 190 degrees vertically - doesnt totally exist I think... pixelY = Mathf.Repeat(pixelY, verticalResolution) + 1f; // make sure is in 360 range, ie from 1 to horizontalResolution
        return pixelY;
    }

    public static Vector2 PositionToLonLat(Vector3 position)
    {
        // from here: https://gamedev.stackexchange.com/questions/149109/calculating-the-latitude-longitude-of-a-raycast-hit-point-on-a-sphere

        // Convert to a unit vector so our y coordinate is in the range -1...1.
        position = Vector3.Normalize(position);

        // The vertical coordinate (y) varies as the sine of latitude
        float lat = Mathf.Asin(position.y) * Mathf.Rad2Deg;

        // Use the 2-argument arctangent, which will correctly handle all four quadrants.
        float lon = Mathf.Atan2(position.z, position.x * -1f) * Mathf.Rad2Deg;
        // Here I'm assuming (0, 0, 1) = 0 degrees longitude, and (1, 0, 0) = +90.
        // You can exchange/negate the components to get a different longitude convention.
        // DREW: I've swapped these so 0,0 is "forward" in the video sphere 

        // I usually put longitude first because I associate vector.x with "horizontal."
        return new Vector2(lon, lat);
    }

    public static Vector2 GetClosestPointOnTarget(AttentionEvent e, Vector2 eyePositionInPixels){
        // rotate the target rectangle until it's centered horizontally in the equirectangular
        var xDiff = (horizontalResolution / 2f) - e.targetHorPixel;
        
        Debug.LogError("xdiff = " + xDiff);

        var targetX = e.targetHorPixel + xDiff;
        var targetY = e.targetVerPixel;

        // rotate the eye points Ex and Ey by the same amount
        var eyeX = eyePositionInPixels.x + xDiff;
        if(eyeX > horizontalResolution) {
            eyeX = eyeX - horizontalResolution; // wrap around;
        }
        var eyeY = eyePositionInPixels.y;

        Debug.LogError("eyeX = " + eyeX);
        Debug.LogError("eyeY = " + eyeY);

        // Calculate the closest point on the target to the eye point
        var largestBoxX = e.targetHorPixel + (e.width / 2f);
        var smallestBoxX = e.targetHorPixel - (e.width / 2f);
        var closestPointOnTargetX = Mathf.Min(largestBoxX, Mathf.Max(smallestBoxX, eyeX));

        var largestBoxY = e.targetVerPixel + (e.height / 2f);
        var smallestBoxY = e.targetVerPixel - (e.height / 2f);
        var closestPointOnTargetY = Mathf.Min(largestBoxY, Mathf.Max(smallestBoxY, eyeY));


        Debug.LogError("closestPointOnTargetX = " + closestPointOnTargetX);
        Debug.LogError("closestPointOnTargetY = " + closestPointOnTargetY);

        // De-rotate this point back into the original frame of reference and return
        var closestPointOnTargetXRotated = closestPointOnTargetX - xDiff;
        if (closestPointOnTargetXRotated < 0){
            closestPointOnTargetXRotated = closestPointOnTargetXRotated + horizontalResolution;
        } else if (closestPointOnTargetXRotated > horizontalResolution) {
            closestPointOnTargetXRotated = closestPointOnTargetXRotated - horizontalResolution;
        }
        return new Vector2(closestPointOnTargetXRotated, closestPointOnTargetY);
    }
}
