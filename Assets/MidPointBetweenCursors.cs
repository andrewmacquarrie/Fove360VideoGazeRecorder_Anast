using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointBetweenCursors : MonoBehaviour
{
    public GameObject left;
    public GameObject right;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Lerp(left.transform.position, right.transform.position, 0.5f);
    }

    Vector3 Lerp(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }
}
