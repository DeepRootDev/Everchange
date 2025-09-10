using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaypointDrive:MonoBehaviour {

	protected Waypoint prevWaypoint = null;
	protected Waypoint myWaypoint = null;
	protected float myTrackLaneOffset = 0.0f;
	protected float percLeftToNextWP = 1.0f;
	protected float totalDistToNextWP = 0.0f;

	protected float turnControl = 0.0f;
	protected float runControl = 0.7f;

	private const float maxHandlingTurnAngle = 80f;
	private bool pathIsClear = true;
	private bool showLinesInSceneView = true;
	private float obstacleSafetyThreshold;
	private Transform[] obstacles;
	private float randomTurningDecisionMaker = 1f;

	Vector3 momentum = Vector3.zero;

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

	private float attackSightRange = 300.0f;

	private static int uniqueID = 0; // just to number at time of spawn for easier identification

	public static void ResetStatics() {
		uniqueID = 0;
	}

	private void Start() {
		name = "Driver #" + (uniqueID++);

		myWaypoint = WayPointManager.instance.startWP;
		prevWaypoint = myWaypoint;
		myWaypoint = prevWaypoint.randNext();

		StartCoroutine(AIbehavior());
	}

	private void FixedUpdate()
	{
		momentum *= 0.94f; // nonlinear, keeping it out of Update to avoid calc
	}
    private void Update()
    {
		if (AInow == AIMode.HumanControl)
		{
			turnControl = moveInput.x;
		}

		transform.Rotate(Vector3.up, turnControl * 180.0f * Time.deltaTime);

		float enginePower = runControl;
		momentum += transform.forward * enginePower * 9.0f * Time.deltaTime;
		Vector3 newPos = transform.position;
		newPos += momentum;
		newPos.y = Vector3.Lerp(myWaypoint.transform.position, prevWaypoint.transform.position, percLeftToNextWP).y;
		transform.position = newPos;
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

	private void Tick()
	{
		SteerTowardPoint(myWaypoint.trackPtForOffset(myTrackLaneOffset));

		Vector3 nextWPTrackLeft = myWaypoint.trackPtForOffset(-1.0f);
		Vector3 nextWPTrackRight = myWaypoint.trackPtForOffset(1.0f);

		Vector3 prevWPTrackLeft = prevWaypoint.trackPtForOffset(-1.0f);
		Vector3 prevWPTrackRight = prevWaypoint.trackPtForOffset(1.0f);

		Vector3 positionLeft = Vector3.Lerp(nextWPTrackLeft, prevWPTrackLeft, percLeftToNextWP);
		Vector3 positionRight = Vector3.Lerp(nextWPTrackRight, prevWPTrackRight, percLeftToNextWP);

		float angleFromLeftEdge = AngleAroundAxis(transform.position - prevWPTrackLeft,
			nextWPTrackLeft - prevWPTrackLeft,Vector3.up);
		if(angleFromLeftEdge > 0.0f) {
			SteerTowardPoint(positionLeft);
		}
		float angleFromRightEdge = AngleAroundAxis(transform.position - prevWPTrackRight,
			nextWPTrackRight - prevWPTrackRight,Vector3.up);
		if(angleFromRightEdge < 0.0f) {
			SteerTowardPoint(positionRight);
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
			Vector3 nextWaypoint = FollowNextWaypoint();

			Vector3 pathToSteerToward = nextWaypoint - transform.position;

			Vector3 localDelta = transform.InverseTransformDirection(pathToSteerToward);

			ShowDebugLines(transform.position, nextWaypoint, Color.yellow);
			ShowDebugLines(transform.position, (transform.position + pathToSteerToward), Color.green);

			float rightTurnAmount = Vector3.Angle(pathToSteerToward, transform.forward);
			rightTurnAmount = rightTurnAmount / 80;
			rightTurnAmount = Mathf.Clamp(rightTurnAmount, 0, maxHandlingTurnAngle);
			if (localDelta.x < -0.001f) { turnControl = turnControl - rightTurnAmount; }
			if (localDelta.x > 0.001f) { turnControl = turnControl + rightTurnAmount; }

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
			AInow != AIMode.FollowTrack || // some other behavior is overriding control
			WayPointManager.instance.levelWayPointList == null) { // no waypoints defined  
			return Vector3.zero; 
		}

		Vector3 gotoPoint = myWaypoint.trackPtForOffset(myTrackLaneOffset);

		gotoPoint.y = transform.position.y; // temporary hack to deal with height inelegantly
		float distTo = Vector3.Distance(transform.position, gotoPoint);
		float closeEnoughToWaypoint = 20.0f;
		percLeftToNextWP = distTo / totalDistToNextWP;

		if(distTo < closeEnoughToWaypoint) {
			prevWaypoint = myWaypoint;
			myWaypoint = myWaypoint.randNext();
			if(AInow != AIMode.HumanControl)
            {
				randomizeTrackLaneOffset();
			}
			totalDistToNextWP = Vector3.Distance(transform.position, myWaypoint.trackPtForOffset(myTrackLaneOffset));
			percLeftToNextWP = 1.0f;
		}

		return gotoPoint;
	}

	protected void randomizeTrackLaneOffset()
	{
		myTrackLaneOffset = Random.Range(-1.0f, 1.0f);
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
