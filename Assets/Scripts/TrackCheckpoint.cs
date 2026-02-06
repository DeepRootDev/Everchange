using System;
using UnityEngine;

public class TrackCheckpoint : MonoBehaviour
{
    LineRenderer visualLineCheckpoint;
    float startingPositionY = -60f;
    public float lineLength = 0;
    public Boolean invertVisualLine;
    public float width = 3;

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
        visualLineCheckpoint.SetPosition(1,new Vector3(lineLength,startingPositionY,0f));
    }
}
