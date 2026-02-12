using System;
using UnityEngine;

public class TrackCheckpoint : MonoBehaviour
{
    LineRenderer visualLineCheckpoint;
    float startingPositionY = -60f;
    public float lineLength = 0;
    public Boolean invertVisualLine;
    public float width = 3;
    private GameObject animatedPulseObject;
    float timer;
    private float duration = 3f;
    Vector3 pulseObjectStartingPosition;
    Vector3 pulseObjectEndPosition;
    float pulseObjectEndPositionXOffset = 2.0f;
    Vector3 lineEndPosition;

    void Start()
    {
        visualLineCheckpoint = GetComponent<LineRenderer>();
        visualLineCheckpoint.positionCount = 2;
        visualLineCheckpoint.startWidth = width;
        visualLineCheckpoint.endWidth = width;
      
        Vector3 startingPosition = new Vector3(0f, startingPositionY, 0f);
        if (invertVisualLine)
        {
            lineLength *= -1;
        }
       
        visualLineCheckpoint.SetPosition(0,startingPosition);
        lineEndPosition = new Vector3(lineLength,startingPositionY,0f);
        visualLineCheckpoint.SetPosition(1,lineEndPosition);
        
        Transform child = transform.Find("animatedPulseObject");
        if (child != null)
        {
            animatedPulseObject = child.gameObject;
            timer = 0f;
            pulseObjectStartingPosition = animatedPulseObject.transform.localPosition;
            pulseObjectEndPosition = pulseObjectStartingPosition + new Vector3((lineEndPosition.x - pulseObjectEndPositionXOffset), 0, 0);
        }

    }

    void Update()
    {
        if (animatedPulseObject != null)
        {
            if (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration); 
                animatedPulseObject.transform.localPosition = Vector3.Lerp(pulseObjectStartingPosition, pulseObjectEndPosition, t);
            }
            else
            {
                ResetAnimatedPulseObject();
            }

        }
    }

    void ResetAnimatedPulseObject()
    {
        animatedPulseObject.SetActive(false);
        animatedPulseObject.transform.position = pulseObjectStartingPosition;
        animatedPulseObject.SetActive(true);
        timer = 0f;
    }
}
