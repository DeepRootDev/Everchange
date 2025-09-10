using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
	public Waypoint[] next;
	public bool inAir = false;
	private long lastRedrawNum=-int.MaxValue;

	void Start() {
		Vector3 pointToward = Vector3.zero;
		for(int ii = 0; ii < next.Length; ii++) {
			pointToward += next[ii].transform.position;
		}
		pointToward /= next.Length;
		//transform.LookAt(pointToward);
	}

	public Waypoint randNext() {
		return next[ Random.Range(0,next.Length) ];
	}
	// assumption: all the points are reasonably in front of you
	public Waypoint nextWPNearestTrackOffset(float laneOffset)
    {
		const float angleTolerance = 0.1f;
		int branches = next.Length;
		// going from -1.0 to 1.0, scaling up to number of lanes, flooring for discrete option, prevent out of bounds
		int k = Mathf.Clamp(Mathf.FloorToInt((laneOffset + 1f) * 0.5f * branches), 0, branches - 1);

		int bestIdx = 0;
		for (int i = 0; i < branches; i++)
		{
			// direction towards this path option
			Vector3 li = transform.InverseTransformPoint(next[i].transform.position);
			float ai = Mathf.Atan2(li.x, li.z);

			// rank angle
			int less = 0;
			for (int j = 0; j < branches; j++)
			{
				if (j != i)
				{
					Vector3 lj = transform.InverseTransformPoint(next[j].transform.position);
					float aj = Mathf.Atan2(lj.x, lj.z);

					bool jIsLeft = (aj < ai - angleTolerance) ||
						(Mathf.Abs(aj - ai) <= angleTolerance && j < i); // tie breaker to avoid edge case
					if (jIsLeft) // counting how many branches are left of me
					{
						less++;
					}
				}
			}
			if (less == k) {
				bestIdx = i;
				break;
			}
		}
		return next[bestIdx];
	}
	public Waypoint nextNum(int i)
	{
		if( i >= next.Length)
        {
			return null;
        }
		return next[i];
	}
	public Waypoint prevPoint() { // implement if we need to support going backwards
		Debug.Log("Didn't code this yet");
		return null;
	}
	public Waypoint pointIsAlong(Vector3 forPt) {
		if(transform.InverseTransformPoint(forPt).z < 0.0f) {
			return null;
		}
		forPt.y = transform.position.y;
		for(int ii = 0; ii < next.Length; ii++) {
			if(next[ii].transform.InverseTransformPoint(forPt).z < 0.0f) {

				Vector3 flatPt = next[ii].transform.position;
				flatPt.y = transform.position.y;
				Vector3 nearestPt = Vector3.Project(forPt - transform.position,
					(flatPt - transform.position).normalized) +
					transform.position;
				float distTo = Vector3.Distance(nearestPt, flatPt);
				float distToPrev = Vector3.Distance(nearestPt, transform.position);
				float totalDist = Vector3.Distance(flatPt, transform.position);

				float sumDiff = (totalDist) - (distTo + distToPrev);
				if(sumDiff < 1.0f && distToPrev/totalDist > 0.0f && distToPrev/totalDist < 1.0f) {
					float widthHere = Mathf.Lerp(transform.localScale.x, next[ii].transform.localScale.x,
						distToPrev/totalDist) * 0.5f;

					if(Vector3.Distance(forPt, nearestPt) < widthHere) {
						return next[ii];
					}
				}
			}
		}
		return null;
	}

	public void drawPathFromHere(long currentRedrawNum)
    {
		if(lastRedrawNum < currentRedrawNum)
        {
			lastRedrawNum = currentRedrawNum; // hack to preventing infinite loops

			Vector3 currWPTrackLeft = trackPtForOffset(-1.0f);
			Vector3 currWPTrackRight = trackPtForOffset(1.0f);

			for (int branch = 0; branch < next.Length; branch++)
			{
				Waypoint nextWP = nextNum(branch);

				Vector3 nextWPTrackLeft = nextWP.trackPtForOffset(-1.0f);
				Vector3 nextWPTrackRight = nextWP.trackPtForOffset(1.0f);

				Debug.DrawLine(currWPTrackLeft, nextWPTrackLeft, Color.green);
				Debug.DrawLine(transform.position, nextWP.transform.position, Color.white);
				Debug.DrawLine(currWPTrackRight, nextWPTrackRight, Color.yellow);

				nextWP.drawPathFromHere(currentRedrawNum);
			}
		}
    }

	// -1.0f is left side of track, 1.0f is right side of track
	public Vector3 trackPtForOffset(float offsetHere) {
		Vector3 trackPerpLine = transform.right;
		float trackWidthHere = transform.localScale.x;
		return transform.position + 0.5f * trackPerpLine * trackWidthHere * offsetHere;
	}
}
