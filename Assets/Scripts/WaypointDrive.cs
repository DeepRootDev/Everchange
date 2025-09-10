using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrive:MonoBehaviour {

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
	[SerializeField] private GameObject headlights;  //assigned in inspector

	public enum AIMode
	{
		FollowTrack,
		ShortTermOverride
	};
	private AIMode AInow = AIMode.FollowTrack;

	private float attackSightRange = 300.0f;

	private static int uniqueID = 0; // just to number at time of spawn for easier identification

	public static void ResetStatics() {
		uniqueID = 0;
	}

	private void Start() {
		name = "Driver #" + (uniqueID++);

		StartCoroutine(AIbehavior());
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
			Vector3 safetyPoint = AvoidObstacles();
			Vector3 pathToSteerToward = (safetyPoint - transform.position) + (nextWaypoint - transform.position);
			ShowDebugLines(transform.position, nextWaypoint, Color.yellow);
			ShowDebugLines(transform.position, safetyPoint, Color.blue);
			ShowDebugLines(transform.position, (transform.position + pathToSteerToward), Color.green);

			if(Player.instance) { // player exists
				RaycastHit rhInfo;
				Vector3 vectorToPlayer = Player.instance.transform.position - transform.position;
				int ignoreLayerMask = 0;
				if (vectorToPlayer.magnitude < attackSightRange) {
					Ray hereToPlayer = new Ray(transform.position, vectorToPlayer);
					if(Physics.Raycast(hereToPlayer, out rhInfo, ignoreLayerMask) == false) { // unobstructed line of sight?
						Debug.Log("line of sign from " +gameObject.name + " to Player, if we care to do something");
					}
				}
				
			}

			float rightTurnAmount = Vector3.Angle(pathToSteerToward, transform.forward);
			rightTurnAmount = rightTurnAmount / 80;
			rightTurnAmount = Mathf.Clamp(rightTurnAmount, 0, maxHandlingTurnAngle);
			float leftTurnAmount = -rightTurnAmount;
			if (pathToSteerToward.x < -0.001f) { turnControl = turnControl - leftTurnAmount; }
			if (pathToSteerToward.x > 0.001f) { turnControl = turnControl + rightTurnAmount; }
			if (pathIsClear == false && turnControl < 0.001f && pathToSteerToward.z < 0) { turnControl = randomTurningDecisionMaker; }
			if (pathIsClear == false && pathToSteerToward.z < 0) { runControl = 0.1f; } else { runControl = 1f; }

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

	private Vector3 AvoidObstacles()
	{
		Vector3 vectorToDestination = Vector3.zero; //transform.forward * obstacleSafetyThreshold;
		Vector3 destinationPoint = transform.position + vectorToDestination;
		pathIsClear = true;
		if(obstacles != null) {
			foreach(Transform obstacle in obstacles) {
				float obstacleDistance = Vector3.Distance(transform.position, obstacle.position);
				if(obstacleDistance < obstacleSafetyThreshold) {
					pathIsClear = false;
					Vector3 vectorToObstacle = obstacle.position - transform.position;
					Vector3 obstaclePoint = transform.position + vectorToObstacle;
					ShowDebugLines(transform.position, obstaclePoint, Color.red);
					Vector3 dirAwayFromObstacle = -vectorToObstacle.normalized;
					Vector3 avoidancePoint = transform.position + (dirAwayFromObstacle * (obstacleSafetyThreshold - obstacleDistance));
					Vector3 vectorToAvoidancePoint = avoidancePoint - transform.position;
					Vector3 newDestinationVector = vectorToDestination + vectorToAvoidancePoint;
					vectorToDestination = newDestinationVector;
					destinationPoint = transform.position + vectorToDestination;
				}
			}
		}
		return destinationPoint;
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
		float closeEnoughToWaypoint = 140.0f;
		percLeftToNextWP = distTo / totalDistToNextWP;

		if(distTo < closeEnoughToWaypoint) {
			prevWaypoint = myWaypoint;
			myWaypoint = myWaypoint.randNext();
			randomizeTrackLaneOffset();
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
