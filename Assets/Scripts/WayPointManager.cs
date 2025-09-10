using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour {
	public static WayPointManager instance;
	public List<Waypoint> levelWayPointList;

	public Waypoint startWP;

	public bool showLinesInSceneView = true;

	public long redrawNum = -int.MaxValue;

	private void Awake()
    {
		instance = this;

		levelWayPointList = new List<Waypoint>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform wpTransform = transform.GetChild(i);
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
