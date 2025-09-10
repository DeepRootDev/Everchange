using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour {
	public static WayPointManager instance;
	public List<Waypoint> levelWayPointList;

	public bool showLinesInSceneView = true;


	private void Awake()
    {
		instance = this;

		/* populating by hand in editor for debug draw
		levelWayPointList = new List<Waypoint>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform wpTransform = transform.GetChild(i);
			levelWayPointList.Add(wpTransform.GetComponent<Waypoint>());
		}*/
	}

	private void OnDrawGizmos()
	{
		Waypoint startWP = levelWayPointList[0];
		Waypoint currentWP = startWP;
		Waypoint nextWP = currentWP.randNext();

		if (showLinesInSceneView)
		{
			Gizmos.color = Color.red;

			do
			{
				Gizmos.DrawWireSphere(startWP.transform.position, 2.0f);

				Vector3 currWPTrackLeft = currentWP.trackPtForOffset(-1.0f);
				Vector3 currWPTrackRight = currentWP.trackPtForOffset(1.0f);

				for (int branch = 0;branch < currentWP.next.Length; branch++)
                {
					nextWP = currentWP.nextNum(branch);

					Vector3 nextWPTrackLeft = nextWP.trackPtForOffset(-1.0f);
					Vector3 nextWPTrackRight = nextWP.trackPtForOffset(1.0f);

					Debug.DrawLine(currWPTrackLeft, nextWPTrackLeft, Color.green);
					Debug.DrawLine(currentWP.transform.position, nextWP.transform.position, Color.white);
					Debug.DrawLine(currWPTrackRight, nextWPTrackRight, Color.yellow);
				}
				currentWP = currentWP.randNext(); // not properly handling all branch drawing yet, just starting
			} while (nextWP != startWP);
		}
	}

}
