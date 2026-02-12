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

    private float waypointMaxDistToTrigger = 30; // FIXME: use a trigger collider instead?
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

    // FIXME: counting how many waypoints everyone has hit is not quite enough
    // what if you go out of order (cheat and cut corners)?
    // what about a tie - we need to compare distance to next wp
    int whatRacePositionAreWe()
    {
        int rank = 1;
        if (thePlayer.hasPassedWaypointNumber < opponent1.hasPassedWaypointNumber 
        || (thePlayer.hasPassedWaypointNumber == opponent1.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent1.distanceToNextWaypoint)
        ) rank++;
        
        if (thePlayer.hasPassedWaypointNumber < opponent2.hasPassedWaypointNumber
        || (thePlayer.hasPassedWaypointNumber == opponent2.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent2.distanceToNextWaypoint)
        ) rank++;
        
        if (thePlayer.hasPassedWaypointNumber < opponent3.hasPassedWaypointNumber
        || (thePlayer.hasPassedWaypointNumber == opponent3.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent3.distanceToNextWaypoint)
        ) rank++;
        
        if (thePlayer.hasPassedWaypointNumber < opponent4.hasPassedWaypointNumber
        || (thePlayer.hasPassedWaypointNumber == opponent4.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent4.distanceToNextWaypoint)
        ) rank++;
        
        return rank;        
    }

    void detectPlayerNearWaypoints()
    {
        Waypoint wp;
        float dist = 0f;
        float minDist = 999999999f;
        int closestOne = -999;
        
        for (int i=0; i< theWaypointManager.levelWayPointList.Count; i++)
        {
            wp = theWaypointManager.levelWayPointList[i];
            if (wp != null)
            {
                dist = Vector3.Distance(wp.transform.position, thePlayer.transform.position);
                if (dist < minDist)
                {
                    closestOne = i;
                    minDist = dist;        
                } // close enough
            } // wp
        } // loop through all WPs

        if (minDist < waypointMaxDistToTrigger)
        {
            if (thePlayer.hasPassedWaypointNumber < closestOne)
            {
                Debug.Log("Player just passed WP#"+closestOne);
                thePlayer.hasPassedWaypointNumber = closestOne;
            }
        }

        // measure remaining dist to next wp
        if (thePlayer.hasPassedWaypointNumber+1 < theWaypointManager.levelWayPointList.Count) {
            wp = theWaypointManager.levelWayPointList[thePlayer.hasPassedWaypointNumber+1];
            thePlayer.distanceToNextWaypoint = Vector3.Distance(wp.transform.position, thePlayer.transform.position);
        } 
        else // must be at the last wp?
        {
            thePlayer.distanceToNextWaypoint = minDist;
        }

    }

    void Update()
    {
        detectPlayerNearWaypoints();

        timeTillNextupdate -= Time.deltaTime;
        if (timeTillNextupdate>0f) return;
        string debugString = "Race Position Debug:\n";
        
        // FIXME TODO
        // compare highest waypoint # reached
        // to break a tie: measure distance to next waypoint
        //int newPosition = UnityEngine.Random.Range(1, 6); // fake
        int newPosition = whatRacePositionAreWe();

        //if (theWaypointManager != null) {
            // then we can loop through this List:
            // theWaypointManager.levelWayPointList
        //}

        debugString += "\nPlayer WP#"+thePlayer.hasPassedWaypointNumber+" dist:"+thePlayer.distanceToNextWaypoint.ToString("F1")+"\n";

        if (opponent1) debugString += "\nOpponent 1 WP#"+opponent1.hasPassedWaypointNumber+" dist:"+opponent1.distanceToNextWaypoint.ToString("F1");
        if (opponent2) debugString += "\nOpponent 2 WP#"+opponent2.hasPassedWaypointNumber+" dist:"+opponent2.distanceToNextWaypoint.ToString("F1");
        if (opponent3) debugString += "\nOpponent 3 WP#"+opponent3.hasPassedWaypointNumber+" dist:"+opponent3.distanceToNextWaypoint.ToString("F1");
        if (opponent4) debugString += "\nOpponent 4 WP#"+opponent4.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");

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
        debugString += "\n\nPlayer distance to finish line: "+dist.ToString("F1");

        if (debugTextGUI != null) {
            debugString += "\nPlayer total distance travelled: "+thePlayer.totalDistanceTravelled.ToString("F2");
            debugString += "\nPlayer Current Race Position: "+currentPosition;
            debugTextGUI.text = debugString;
        }

    }
}
