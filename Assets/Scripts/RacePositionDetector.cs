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

    [Header("How far away from player until AI slows/speeds")]
    public float rubberbandingStartDistance = 50f;

    [Header("The Racers! (optional)")]
    public PlayerMovement thePlayer;
    public WaypointDrive opponent1;
    public WaypointDrive opponent2;
    public WaypointDrive opponent3;
    public WaypointDrive opponent4;
    public WaypointDrive opponent5;
    public WaypointDrive opponent6;
    public WaypointDrive opponent7;
    public WaypointDrive opponent8;
    public WaypointDrive opponent9;
    public WaypointDrive opponent10;

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
        float tooFar = rubberbandingStartDistance; // once farther than this from player bots start rubberbanding

        if (opponent1) {
            bool rubberband1 = Vector3.Distance(opponent1.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent1.hasPassedWaypointNumber 
            || (thePlayer.hasPassedWaypointNumber == opponent1.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent1.distanceToNextWaypoint)
            ) {
                if (rubberband1) opponent1.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband1) opponent1.pleaseSpeedUp();
            }
        }
        
        if (opponent2) {
            bool rubberband2 = Vector3.Distance(opponent2.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent2.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent2.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent2.distanceToNextWaypoint)
            ) {
                if (rubberband2) opponent2.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband2) opponent2.pleaseSpeedUp();
            }
        }
        
        if (opponent3) {
            bool rubberband3 = Vector3.Distance(opponent3.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent3.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent3.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent3.distanceToNextWaypoint)
            ) {
                if (rubberband3) opponent3.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband3) opponent3.pleaseSpeedUp();
            }
        }
        
        if (opponent4) {
            bool rubberband4 = Vector3.Distance(opponent4.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent4.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent4.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent4.distanceToNextWaypoint)
            ) {
                if (rubberband4) opponent4.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband4) opponent4.pleaseSpeedUp();
            }
        }

        if (opponent5) {
            bool rubberband5 = Vector3.Distance(opponent5.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent5.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent5.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent5.distanceToNextWaypoint)
            ) {
                if (rubberband5) opponent5.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband5) opponent5.pleaseSpeedUp();
            }
        }

        if (opponent6) {
            bool rubberband6 = Vector3.Distance(opponent6.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent6.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent6.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent6.distanceToNextWaypoint)
            ) {
                if (rubberband6) opponent6.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband6) opponent6.pleaseSpeedUp();
            }
        }

        if (opponent7) {
            bool rubberband7 = Vector3.Distance(opponent7.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent7.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent7.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent7.distanceToNextWaypoint)
            ) {
                if (rubberband7) opponent7.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband7) opponent7.pleaseSpeedUp();
            }
        }

        if (opponent8) {
            bool rubberband8 = Vector3.Distance(opponent8.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent8.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent8.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent8.distanceToNextWaypoint)
            ) {
                if (rubberband8) opponent8.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband8) opponent8.pleaseSpeedUp();
            }
        }


        if (opponent9) {
            bool rubberband9 = Vector3.Distance(opponent9.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent9.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent9.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent9.distanceToNextWaypoint)
            ) {
                if (rubberband9) opponent9.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband9) opponent9.pleaseSpeedUp();
            }
        }


        if (opponent10) {
            bool rubberband10 = Vector3.Distance(opponent10.transform.position,thePlayer.transform.position) > tooFar;
            if (thePlayer.hasPassedWaypointNumber < opponent10.hasPassedWaypointNumber
            || (thePlayer.hasPassedWaypointNumber == opponent10.hasPassedWaypointNumber && thePlayer.distanceToNextWaypoint > opponent10.distanceToNextWaypoint)
            ) {
                if (rubberband10) opponent10.pleaseSlowDown();
                rank++;
            } else {
                if (rubberband10) opponent10.pleaseSpeedUp();
            }
        }

        return rank;        
    }

    int calculateWaypointNumber(Waypoint myWaypoint)
    {
        int wpNum = WayPointManager.instance.levelWayPointList.IndexOf(myWaypoint);
        //Debug.Log("Found player waypoint: "+wpNum);
        return wpNum;
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
            // does not account for branching paths
            //if (thePlayer.hasPassedWaypointNumber < closestOne)
            //{
            //    Debug.Log("Player just passed WP#"+closestOne);
            //    thePlayer.hasPassedWaypointNumber = closestOne;
            //}

            wp = theWaypointManager.levelWayPointList[closestOne];
            thePlayer.hasPassedWaypointNumber = calculateWaypointNumber(wp);
        }

        // measure remaining dist to next wp
        if (thePlayer.hasPassedWaypointNumber+1 < theWaypointManager.levelWayPointList.Count) {
            //Debug.Log("DEBUG: thePlayer.hasPassedWaypointNumber="+thePlayer.hasPassedWaypointNumber+" theWaypointManager.levelWayPointList.Count="+theWaypointManager.levelWayPointList.Count);
            wp = theWaypointManager.levelWayPointList[thePlayer.hasPassedWaypointNumber+1];
            //Debug.Log("DEBUG: wp = ",wp);
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
        if (opponent5) debugString += "\nOpponent 5 WP#"+opponent5.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");
        if (opponent6) debugString += "\nOpponent 6 WP#"+opponent6.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");
        if (opponent7) debugString += "\nOpponent 7 WP#"+opponent7.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");
        if (opponent8) debugString += "\nOpponent 8 WP#"+opponent8.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");
        if (opponent9) debugString += "\nOpponent 9 WP#"+opponent9.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");
        if (opponent10) debugString += "\nOpponent 10 WP#"+opponent10.hasPassedWaypointNumber+" dist: "+opponent4.distanceToNextWaypoint.ToString("F1");

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
                case 11: updateThisText.text = "11th"; break;
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
