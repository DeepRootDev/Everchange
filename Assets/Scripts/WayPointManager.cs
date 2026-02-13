using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour {
	public static WayPointManager instance;

    [Header("Drag the sky or ground track group here:")]
    public GameObject levelWaypointsToUse;

    [Header("Drag the starting waypoint from that group here:")]
	public Waypoint startWP;

    [Header("(do not edit) Automatically filled at init:")]
    public List<Waypoint> levelWayPointList;
   

	public bool showLinesInSceneView = true;

	private long redrawNum = -int.MaxValue;

	private void Awake()
    {
		instance = this;

        if (levelWaypointsToUse==null)
        {
            // use 1st track if we forgot to fill this in inthe inspector
            Debug.Log("You forgot to fill in the waypoint manager's levelWaypointsToUse field - assuming the 1st track!");
            levelWaypointsToUse = transform.GetChild(0).gameObject;
        }

		levelWayPointList = new List<Waypoint>();
		for (int i = 0; i < levelWaypointsToUse.transform.childCount; i++)
		{
			Transform wpTransform = levelWaypointsToUse.transform.GetChild(i);
			levelWayPointList.Add(wpTransform.GetComponent<Waypoint>());
		}
	}

	private void OnDrawGizmos()
	{
		Waypoint currentWP = startWP;
		Waypoint nextWP = currentWP.randNext();

		redrawNum++;
		if (showLinesInSceneView)
		{
			Gizmos.color = Color.red;
			startWP.drawPathFromHere(redrawNum);
		}
	}

}
