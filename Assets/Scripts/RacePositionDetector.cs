using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

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
        // please ensure these are filled in in the inspector!
        if (!theWaypointManager) Debug.LogError("RacePositiondetector needs a waypointManager");
        if (!thePlayer) Debug.LogError("RacePositiondetector needs player");
        if (!theFinishLine) Debug.LogError("RacePositiondetector needs a finish line");
        if (!updateThisText) Debug.LogError("RacePositiondetector needs a text control");
    }

    int whatRacePositionAreWe()
    {
        int rank = 1;
        if (thePlayer.hasPassedWaypointNumber < opponent1.hasPassedWaypointNumber) rank++;
        if (thePlayer.hasPassedWaypointNumber < opponent2.hasPassedWaypointNumber) rank++;
        if (thePlayer.hasPassedWaypointNumber < opponent3.hasPassedWaypointNumber) rank++;
        if (thePlayer.hasPassedWaypointNumber < opponent4.hasPassedWaypointNumber) rank++;
        return rank;        
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

        //if (theWaypointManager != null) {
            // then we can loop through this List:
            // theWaypointManager.levelWayPointList
        //}

        debugString += "\nPlayer hit waypoint: "+thePlayer.hasPassedWaypointNumber;
        debugString += "\nPlayer next wp dist: "+thePlayer.distanceToNextWaypoint.ToString("F2")+"\n";

        if (opponent1) debugString += "\nOpponent 1 hit waypoint: "+opponent1.hasPassedWaypointNumber;
        if (opponent1) debugString += "\nOpponent 1 next wp dist: "+opponent1.distanceToNextWaypoint.ToString("F2");
        if (opponent2) debugString += "\nOpponent 2 hit waypoint: "+opponent2.hasPassedWaypointNumber;
        if (opponent2) debugString += "\nOpponent 2 next wp dist: "+opponent2.distanceToNextWaypoint.ToString("F2");
        if (opponent3) debugString += "\nOpponent 3 hit waypoint: "+opponent3.hasPassedWaypointNumber;
        if (opponent3) debugString += "\nOpponent 3 next wp dist: "+opponent3.distanceToNextWaypoint.ToString("F2");
        if (opponent4) debugString += "\nOpponent 4 hit waypoint: "+opponent4.hasPassedWaypointNumber;
        if (opponent4) debugString += "\nOpponent 4 next wp dist: "+opponent4.distanceToNextWaypoint.ToString("F2");


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

        float dist = Vector3.Distance(thePlayer.transform.position,theFinishLine.transform.position);
        debugString += "\n\nPlayer distance to finish line: "+dist.ToString("F2");

        if (debugTextGUI != null) {
            debugString += "\nPlayer total distance travelled: "+thePlayer.totalDistanceTravelled.ToString("F2");
            debugString += "\nPlayer Race Position: "+currentPosition;
            debugTextGUI.text = debugString;
        }

    }
}
