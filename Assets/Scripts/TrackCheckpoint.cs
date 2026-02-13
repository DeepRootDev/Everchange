using System;
using Unity.VisualScripting;
using UnityEngine;

public class TrackCheckpoint : MonoBehaviour
{
    LineRenderer visualLineCheckpoint;
    public float lineLength = 0;
    public float width = 3;
    public Transform TheNextCheckpoint;

    void Start()
    {

        if (TheNextCheckpoint)
        {
            visualLineCheckpoint = GetComponent<LineRenderer>();
            visualLineCheckpoint.positionCount = 2;
            visualLineCheckpoint.startWidth = width;
            visualLineCheckpoint.endWidth = width;
      
            Transform anchorObj = transform.Find("anchorLinePosition");
            visualLineCheckpoint.SetPosition(0,anchorObj.position);
            
            Transform checkPointAnchor = TheNextCheckpoint.transform.Find("anchorLinePosition");            
            visualLineCheckpoint.SetPosition(1,checkPointAnchor.position);
    
        }
        else
        {
            Transform arrow = transform.Find("animatedPulseObject");
            if (arrow != null)
            {
                arrow.gameObject.SetActive(false);
            }
        }

    }
}
