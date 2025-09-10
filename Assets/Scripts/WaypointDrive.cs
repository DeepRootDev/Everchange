using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaypointDrive:MonoBehaviour {
	private float runspeed = 90.0f;
	float lateralSpeed = 12.5f;

	private Waypoint prevWaypoint = null;
	private Waypoint myWaypoint = null;
	private float myTrackLaneOffset = 0.0f;
	private float myTrackLaneOffsetAITarget = 0.0f;
	private float percLeftToNextWP = 1.0f;
	private float totalDistToNextWP = 0.0f;

	private float turnControl = 0.0f;
	private float runControl = 0.7f;

	private const float maxHandlingTurnAngle = 80f;
	private bool showLinesInSceneView = true;
	private float obstacleSafetyThreshold;
	private Transform[] obstacles;
	private float randomTurningDecisionMaker = 1f;

	Vector3 lookAheadPt;

	Vector2 moveInput;

	public void OnMove(InputAction.CallbackContext ctx)
	{
		moveInput = ctx.ReadValue<Vector2>();
	}

	public enum AIMode
	{
		FollowTrack,
		ShortTermOverride,
		HumanControl // just doing player character as special case of AI driver, for debugging by switching control etc
	};
	public AIMode AInow = AIMode.FollowTrack;

	private void Start() {
		myWaypoint = WayPointManager.instance.startWP;
		prevWaypoint = myWaypoint;
		myWaypoint = prevWaypoint.randNext();

		StartCoroutine(AIbehavior());
	}

    private void FixedUpdate()
    {
		transform.rotation = Quaternion.Slerp(transform.rotation,
			Quaternion.LookRotation(lookAheadPt - transform.position), 0.2f);
    }

    private void Update()
    {
		Vector3 nextWPTrackLeft = myWaypoint.trackPtForOffset(-1.0f);
		Vector3 nextWPTrackRight = myWaypoint.trackPtForOffset(1.0f);

		Vector3 prevWPTrackLeft = prevWaypoint.trackPtForOffset(-1.0f);
		Vector3 prevWPTrackRight = prevWaypoint.trackPtForOffset(1.0f);

		Vector3 positionLeft = Vector3.Lerp(nextWPTrackLeft, prevWPTrackLeft, percLeftToNextWP);
		Vector3 positionRight = Vector3.Lerp(nextWPTrackRight, prevWPTrackRight, percLeftToNextWP);

		if (AInow != AIMode.HumanControl)
		{
			float laneGoalDelta = myTrackLaneOffset - myTrackLaneOffsetAITarget;
			if(Mathf.Abs(laneGoalDelta) > 0.1f)
            {
				moveInput.x = (myTrackLaneOffset < myTrackLaneOffsetAITarget ? 1.0f : -1.0f) * 0.7f;
			} else
            {
				moveInput.x *= 0.8f; // technically not frame rate safe in update, I don't think it'll matter here
			}
		}
		float trackWidthHere = Vector3.Distance(positionLeft, positionRight);
		myTrackLaneOffset += moveInput.x * lateralSpeed * (2f / Mathf.Max(trackWidthHere, 1e-4f)) * Time.deltaTime;
		myTrackLaneOffset = Mathf.Clamp(myTrackLaneOffset, -1.0f, 1.0f);

		// transform.Rotate(Vector3.up, turnControl * 180.0f * Time.deltaTime);

		float enginePower = runControl * runspeed;
		Vector3 newPos = transform.position;

		float WPSegmentLength = Vector3.Distance(myWaypoint.transform.position, prevWaypoint.transform.position);
		if (WPSegmentLength > 0f)
		{
			percLeftToNextWP -= (enginePower / WPSegmentLength) * Time.deltaTime;
			if(percLeftToNextWP <0f)
            {
				// advance to next waypoint
				AdvanceWP();
			}
		}
		else
		{
			Debug.LogWarning("Waypoints overlapped, error divide by zero avoided " + myWaypoint.name + ", " + prevWaypoint.name);
		}
		float trackLeftRightNormalized = (myTrackLaneOffset + 1.0f) * 0.5f; // math from -1 to 1 into 0.0-1.0
		transform.position = Vector3.Lerp(positionLeft, positionRight, trackLeftRightNormalized);
		lookAheadPt = Vector3.Lerp(nextWPTrackLeft, nextWPTrackRight, trackLeftRightNormalized);
	}

	float heightUnderMe(Vector3 atPos)
	{
		int ignoreMask = 0;
		float lookdownFromAboveHeight = 2.0f;
		RaycastHit rhInfo;
		if (Physics.Raycast(atPos + Vector3.up * lookdownFromAboveHeight,
			-Vector3.up * lookdownFromAboveHeight, out rhInfo, 8.0f, ignoreMask))
		{
			return rhInfo.point.y;
		}
		else
		{
			return lookdownFromAboveHeight; // nothing underneath us
		}
	}

	// helper function borrowed from https://forum.unity3d.com/threads/turn-left-or-right-to-face-a-point.22235/
	private float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA = dirA - Vector3.Project(dirA, axis);
		dirB = dirB - Vector3.Project(dirB, axis);
		float angle = Vector3.Angle(dirA, dirB);
		return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
	}

	IEnumerator AIbehavior() {
		while (true) {

			if (myWaypoint == null) {
				Debug.Log("no waypoint data found, AI bailing now");
				yield break;
			}
			if (Random.Range(1, 6) == 1) { randomTurningDecisionMaker = randomTurningDecisionMaker * -1; }
			ResetDefaultDrivingControls();

			yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
		}
	}

	private void ResetDefaultDrivingControls()
	{
		runControl = 0.5f;
		turnControl = 0.0f;
	}

	private void ShowDebugLines(Vector3 startPoint, Vector3 endPoint, Color color)
	{
		if (showLinesInSceneView)
		{
		Debug.DrawLine(startPoint, endPoint, color);                //All debug lines are centralized here so we can turn this on and off by adjusting the bool
		}
	}

	private void OnDrawGizmos()
	{
		if (showLinesInSceneView)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, obstacleSafetyThreshold);
		}
	}
	
	Vector3 FollowNextWaypoint()
	{ // returns a Waypoint
		if(myWaypoint == null || // no waypoints were found in level
			AInow == AIMode.ShortTermOverride || // some other behavior is overriding control
			WayPointManager.instance.levelWayPointList == null) { // no waypoints defined  
			return Vector3.zero; 
		}

		return myWaypoint.trackPtForOffset(myTrackLaneOffset);
	}

	void AdvanceWP()
    {
		prevWaypoint = myWaypoint;
		int nextWPCount = myWaypoint.next.Length;
		if (nextWPCount > 1)
        {
			myWaypoint = myWaypoint.nextWPNearestTrackOffset(myTrackLaneOffset);
		}
		else
        {
			myWaypoint = myWaypoint.next[0];
		}

		if (AInow != AIMode.HumanControl)
		{
			randomizeTrackLaneOffset();
		}
		totalDistToNextWP = Vector3.Distance(transform.position, myWaypoint.trackPtForOffset(myTrackLaneOffset));
		percLeftToNextWP = 1.0f;
	}

	private void randomizeTrackLaneOffset()
	{
		myTrackLaneOffsetAITarget = Random.Range(-1.0f, 1.0f);
	}

	// currently only aims for waypoint in ordered track maps, but could also point to targeted craft, or generated destination
	void SteerTowardPoint(Vector3 driveToPt) {
		float turnAmt = AngleAroundAxis(transform.forward,
			driveToPt - transform.position,Vector3.up);
		float angDeltaForGentleTurn = 10.0f;
		float angDeltaForSharpTurn = 30.0f;
		float gentleTurn = 0.5f;
		float sharpTurn = 1.0f;
		float gentleTurnEnginePower = 0.9f;
		float sharpTurnEnginePower = 0.6f;

		if(turnAmt < -angDeltaForSharpTurn) {
			turnControl = -sharpTurn;
			runControl = sharpTurnEnginePower;
		} else if(turnAmt > angDeltaForSharpTurn) {
			turnControl = sharpTurn;
			runControl = sharpTurnEnginePower;
		} else if(turnAmt < -angDeltaForGentleTurn) {
			turnControl = -gentleTurn;
			runControl = gentleTurnEnginePower;
		} else if(turnAmt > angDeltaForGentleTurn) {
			turnControl = gentleTurn;
			runControl = gentleTurnEnginePower;
		} else {
			turnControl = 0.0f;
			runControl = 1.0f;
		}
		ShowDebugLines(transform.position, driveToPt, Color.cyan);
	}
	
}
