using UnityEngine;
using TMPro;
using System;

public class RacePositionDetector : MonoBehaviour
{
    public TextMeshProUGUI updateThisText;
    public float updateDelay = 0.5f;
    public PlayerMovement thePlayer;
    public WaypointDrive opponent1;
    public WaypointDrive opponent2;
    public WaypointDrive opponent3;
    public WaypointDrive opponent4;

    private int currentPosition = -1;
    private float timeTillNextupdate = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        if (updateThisText==null) return;
        timeTillNextupdate -= Time.deltaTime;
        if (timeTillNextupdate>0f) return;

        // FIXME TODO
        // compare highest waypoint # reached
        // to break a tie: measure distance to next waypoint
        int newPosition = UnityEngine.Random.Range(1, 6); // faked for now

        if (newPosition != currentPosition) { // changed?
            switch (newPosition) {
                case 1: updateThisText.text = "1st"; break;
                case 2: updateThisText.text = "2nd"; break;
                case 3: updateThisText.text = "3rd"; break;
                case 4: updateThisText.text = "4th"; break;
                case 5: updateThisText.text = "5th"; break;
                case 6: updateThisText.text = "6th"; break;
                case 7: updateThisText.text = "7th"; break;
                case 8: updateThisText.text = "8th"; break;
                case 9: updateThisText.text = "9th"; break;
                case 10: updateThisText.text = "10th"; break;
                default: updateThisText.text = "1st"; break;
            }
            currentPosition = newPosition;
        }

        timeTillNextupdate = updateDelay;

    }
}
