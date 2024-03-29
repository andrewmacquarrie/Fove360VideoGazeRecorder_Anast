﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideTargetAreaIdentifier : MonoBehaviour
{
    public GameObject eyeLocation;
    public GameObject target;

    private AttentionEvent e;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(this.name + ": " + InsideTargetArea());
    }

    public void Clear()
    {
        e = null;
    }

    public void SetAttentionEvent(AttentionEvent ev)
    {
        e = ev;
    }

    public bool InsideTargetArea()
    {
        if (e == null)
        {
            // no targets happened yet
            return false;
        }

        // get the pixel coords for the target and eye positions
        var targetPixelCoords = AngleHelperMethods.PositionToPixelCoords(target.transform.position);
        var eyePixelCoords = AngleHelperMethods.PositionToPixelCoords(eyeLocation.transform.position);

        // calculate the pixel coord for the closest point in the target to the eye position
        var closestPointOnTarget = AngleHelperMethods.GetClosestPointOnTarget(e, eyePixelCoords);

        // calculate the long/lat for that pixel coord
        var closestPointOnTargetLonLat = new Vector2(AngleHelperMethods.PixelCoordToLong(closestPointOnTarget.x), AngleHelperMethods.PixelCoordToLat(closestPointOnTarget.y));

        // get long/lat as rays?
        var vectorToClosestPointOnTarget = AngleHelperMethods.LonLatToPosition(closestPointOnTargetLonLat.x, closestPointOnTargetLonLat.y);

        // show this on the screen as a ray
        //Debug.DrawRay(new Vector3(0,0,0), vectorToClosestPointOnTarget, Color.green, 1f);

        // claculate 3d angle between these, and add deactivationAngle to that - if less than this, deactivate object
        var vectorToTarget = target.transform.position;
        var angleTargetCentreToClosestTargetPoint = Vector3.Angle(vectorToTarget, vectorToClosestPointOnTarget);
        var vectorToObjectDeactivationIsBasedOn = eyeLocation.transform.position; // unlike deactive, should consider "inside" based on eye location only

        if (Vector3.Angle(vectorToObjectDeactivationIsBasedOn, vectorToTarget) < angleTargetCentreToClosestTargetPoint)
        {
            return true;
        }
        return false;
    }
}
