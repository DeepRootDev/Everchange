using System;
using Unity.VisualScripting;
using UnityEngine;

public class TrackCheckpoint : MonoBehaviour
{
    LineRenderer visualLineCheckpoint;
    public float lineLength = 0;
    public float width = 3;
    public Transform TheNextCheckpoint;
    private GameObject animatedPulseObject;
    float timer;
    private float duration = 20f;
    Vector3 pulseObjectStartingPosition;
    Vector3 lineEndPosition;

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
            lineEndPosition = checkPointAnchor.position;
            
            visualLineCheckpoint.SetPosition(1,lineEndPosition);
            
            //TODO:FIXME
            // Transform arrow = transform.Find("animatedPulseObject");
            // if (arrow != null)
            // {
            //     animatedPulseObject = arrow.gameObject;
            //     timer = 0f;
            //     pulseObjectStartingPosition = animatedPulseObject.transform.localPosition;
            //     pulseObjectEndPosition = pulseObjectStartingPosition + lineEndPosition;
            // }
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

    void Update()
    {
        //TODO: FIXME
        // if (TheNextCheckpoint != null)
        // {
        //     if (timer < duration)
        //     {
        //         timer += Time.deltaTime;
        //         float t = Mathf.Clamp01(timer / duration); 
        //         animatedPulseObject.transform.localPosition = Vector3.Lerp(pulseObjectStartingPosition, pulseObjectEndPosition, t);
        //     }
        //     else
        //     {
        //         ResetAnimatedPulseObject();
        //     }

        // }
    }

    void ResetAnimatedPulseObject()
    {
        animatedPulseObject.SetActive(false);
        animatedPulseObject.transform.position = pulseObjectStartingPosition;
        animatedPulseObject.SetActive(true);
        timer = 0f;
    }
}
