using UnityEngine;
using TMPro;
using System;

public class RacePositionDetector : MonoBehaviour
{
    [Header("The GUI with 1st, 2nd, 3rd:")]
    public TextMeshProUGUI updateThisText;

    [Header("Debug Info Goes Here:")]
    public TextMeshProUGUI debugTextGUI;

    [Header("Update interval in seconds:")]
    public float updateDelay = 0.5f;

    [Header("To get the list of waypoints:")]
    public WayPointManager theWaypointManager;

    [Header("The position of the finish line:")]
    public Transform theFinishLine;

    [Header("The Racers!")]
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
        timeTillNextupdate -= Time.deltaTime;
        if (timeTillNextupdate>0f) return;
        string debugString = "Race Position Debug:\n";
        
        // FIXME TODO
        // compare highest waypoint # reached
        // to break a tie: measure distance to next waypoint
        int newPosition = UnityEngine.Random.Range(1, 6); // faked for now

        if (theWaypointManager != null) {
            // then we can loop through this List:
            // theWaypointManager.levelWayPointList
        }

        if (theFinishLine != null && thePlayer != null)
        {
            float dist = Vector3.Distance(thePlayer.transform.position,theFinishLine.transform.position);
            debugString += "\nDistance to finish line: "+dist.ToString("F2");
        }

        if (thePlayer != null)
        {
            debugString += "\nHighest waypoint reached: "+thePlayer.hasPassedWaypointNumber;
            debugString += "\nDistance to next waypoint: "+thePlayer.distanceToNextWaypoint.ToString("F2");
        }

        if (updateThisText != null && newPosition != currentPosition) { // changed?
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

        if (debugTextGUI != null) {
            debugString += "\nCurrent Race Position: "+currentPosition;
            debugString += "\nDistance Travelled: "+thePlayer.totalDistanceTravelled.ToString("F2");
            debugTextGUI.text = debugString;
        }

    }
}
