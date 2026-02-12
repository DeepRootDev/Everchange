using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

// NOTE(marvin): This component is the closest thing to an EnemyAI component. I integrated code for obstacle activation
// into this component. Admittedly not related at all to the name of the component. Perhaps should move AI obstacle
// activation out or rename the component. Regions of code that has to do with obstacle activation is wrapped in
// #region OBSTACLE_ACTIVATION.

public class WaypointDrive:MonoBehaviour {

	public float defaultSpeed = 80.0f;
	public float currentSpeed = 0;

	private float flySpeed = 130.0f;
	float lateralSpeed = 12.5f;
	float flyVerticalSpeed = 0.5f;
	float flyRange = 18.0f;

	public bool inAir = false; // public so that PlayerSounds knows about it

	private Waypoint prevWaypoint = null;
	private Waypoint myWaypoint = null;
	private float myTrackLaneOffset = 0.0f;
	private float myTrackLaneOffsetAITarget = 0.0f;
	public float percLeftToNextWP = 1.0f;
    public float distanceToNextWaypoint = 0f;
	private float totalDistToNextWP = 0.0f;
    public int hasPassedWaypointNumber = 0;

	private float turnControl = 0.0f;
	private float runControl = 0.7f;

	private const float maxHandlingTurnAngle = 80f;
	private bool showLinesInSceneView = true;
	private float obstacleSafetyThreshold;
	private Transform[] obstacles;
	private float randomTurningDecisionMaker = 1f;

	ParticleSystem feetDust;

	Vector3 lookAheadPt;

	Vector2 moveInput;
	float verticalOffset = 0.0f;

	public bool distruptionFromGreenPowerUpActive = false;
	[SerializeField] private float maxTimeForGreenPowerUp = 2.0f;
	private float timeLeftForGreenPowerUp;
    [SerializeField] private float greenPowerUpDistruptionMagnitude = 20.0f;
	private float currentDistruptionFromOutsideFactors = 0.0f;
	
	#region OBSTACLE_ACTIVATION

	private Transform playerTransform;
	private readonly HashSet<ActivatorArea> withinActivatorAreas = new();
	private readonly float checkObstacleActivationEveryXSeconds = 0.5f;
	private float checkObstacleActivationTimeAccumulator = 0.0f;
	#endregion

    private void Awake()
    {
		currentSpeed = defaultSpeed;
    }

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
		feetDust = GetComponent<ParticleSystem>();
		if(WayPointManager.instance) {
			myWaypoint = WayPointManager.instance.startWP;
		} else {
			Debug.Log("WaypointDrive couldn't find WayPointManager.instance.startWP - turning off character");
			enabled = false;
			return;
		}
		prevWaypoint = myWaypoint;
		UpdateAirOrGroundState();
		myWaypoint = prevWaypoint.randNext();

		// FIXME: maybe we need to do this:
        // if (AInow != AIMode.HumanControl)
        StartCoroutine(AIbehavior());

        #region OBSTACLE_ACTIVATION
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerGo.transform;
        #endregion
	}

    


    private void UpdateAirOrGroundState()
    {
        // FIXME: for non AI players we might need
        // to use heightUnderMe() function rather than waypoint data

		if(inAir)
        {
			inAir = prevWaypoint.inAir; // turn off upon landing
		} else
        {
			inAir = myWaypoint.inAir; // turn on upon leaving ground
		}
		ParticleSystem.EmissionModule emission = feetDust.emission;

		emission.rateOverTime = (inAir ? 0 : 200);
	}

	private void FixedUpdate()
    {
	    // NOTE(marvin): If on ground, only looking left and right is relevant, which is the Y axis.
	    Quaternion targetRotation = Quaternion.LookRotation(lookAheadPt - transform.position);
	    if (!inAir)
	    {
		    targetRotation.x = 0;
		    targetRotation.z = 0;
	    }
		transform.rotation = Quaternion.Slerp(transform.rotation,
			targetRotation, 0.2f);

		#region OBSTACLE_ACTIVATION
	    // NOTE(marvin): Every frame-independent amount of time has passed, the AI checks whether should activate an
	    // obstacle. The closer to the player the more likely.
	    checkObstacleActivationTimeAccumulator += Time.fixedDeltaTime;
	    if (checkObstacleActivationTimeAccumulator >= checkObstacleActivationEveryXSeconds)
	    {
		    checkObstacleActivationTimeAccumulator -= checkObstacleActivationEveryXSeconds;
		    if (withinActivatorAreas.Count > 0 && ShouldActivateObstacle())
		    {
			    // NOTE(marvin): Not terribly efficient, but in practice, there's going to be 1 area only. 
			    var areaToActivate = withinActivatorAreas.ElementAt(Random.Range(0, withinActivatorAreas.Count));
			   // areaToActivate.Activate();
		    }
	    }

	    #endregion
    }

	private bool ShouldActivateObstacle()
	{
		const float baseChance = 0.05f;

		const float maximumChanceBonus = 0.1f;
		const float maximumDistance = 300.0f;

		float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
		float bonusChance = (1.0f - Mathf.Clamp(distanceToPlayer / maximumDistance, 0.0f, 1.0f)) * maximumChanceBonus;
		float chance = baseChance + bonusChance;
		
		// NOTE(marvin): The fact that 1 is excluded means that probabilities aren't represented properly, but whatever.
		var diceRoll = Random.Range(0.0f, 1.0f);
		var shouldActivateObstacle = diceRoll <= chance;
		return shouldActivateObstacle;
	}


    private void Update()
    {
		CheckAndUpdateGreenPowerUp();
        if (!myWaypoint)
        {
            //Debug.Log("ERROR in WaypointDrive.Update(): myWaypoint is null. Maybe the WayPointManager has no startWP?");
            // FIXME: maybe search for the right wp entity by name
            return; // do nothing! 
        }
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

		if(inAir)
        {
			verticalOffset += moveInput.y * flyVerticalSpeed * Time.deltaTime;
			verticalOffset = Mathf.Clamp(verticalOffset, -1.0f, 1.0f);

		} else
        {
			verticalOffset = 0.0f;
		}

		float trackWidthHere = Vector3.Distance(positionLeft, positionRight);
		myTrackLaneOffset += moveInput.x * lateralSpeed * (2f / Mathf.Max(trackWidthHere, 1e-4f)) * Time.deltaTime;
		myTrackLaneOffset = Mathf.Clamp(myTrackLaneOffset, -1.0f, 1.0f);

		// transform.Rotate(Vector3.up, turnControl * 180.0f * Time.deltaTime);

		float enginePower = runControl * (inAir ? flySpeed : currentSpeed);
		Vector3 newPos = transform.position;

		float WPSegmentLength = Vector3.Distance(myWaypoint.transform.position, prevWaypoint.transform.position);
		if (WPSegmentLength > 0f)
		{
			if (Mathf.Abs(currentDistruptionFromOutsideFactors) < 0.1f)
			{
                percLeftToNextWP -= (enginePower / WPSegmentLength) * Time.deltaTime;
            }

            distanceToNextWaypoint = Vector3.Distance(transform.position, myWaypoint.transform.position);

			if(percLeftToNextWP <0f)
            {
				// advance to next waypoint
				AdvanceWP();
				UpdateAirOrGroundState();
			}
		}
		else
		{
			Debug.LogWarning("Waypoints overlapped, error divide by zero avoided " + myWaypoint.name + ", " + prevWaypoint.name);
		}
		float trackLeftRightNormalized = (myTrackLaneOffset + 1.0f) * 0.5f; // math from -1 to 1 into 0.0-1.0
		// NOTE(marvin): If on ground, only changes x and z so that Unity's gravity physics can continue to work on y.
		var targetPosition = Vector3.Lerp(positionLeft, positionRight, trackLeftRightNormalized) + Vector3.up* verticalOffset * flyRange;
		transform.position = (inAir ? targetPosition : new Vector3(targetPosition.x, transform.position.y, targetPosition.z)) + new Vector3(0, currentDistruptionFromOutsideFactors, 0);
		lookAheadPt = Vector3.Lerp(nextWPTrackLeft, nextWPTrackRight, trackLeftRightNormalized);
    }

    private void CheckAndUpdateGreenPowerUp()
    {
		if (distruptionFromGreenPowerUpActive)
		{
			currentDistruptionFromOutsideFactors -= greenPowerUpDistruptionMagnitude * Time.deltaTime / maxTimeForGreenPowerUp;
			if (currentDistruptionFromOutsideFactors <= 0)
			{
				currentDistruptionFromOutsideFactors = 0;
            }
            timeLeftForGreenPowerUp -= Time.deltaTime;
			if (timeLeftForGreenPowerUp <= 0)
			{
				distruptionFromGreenPowerUpActive = false;
			}
		}
		else 
		{
			currentDistruptionFromOutsideFactors = 0.0f;
            timeLeftForGreenPowerUp = maxTimeForGreenPowerUp;
        }
    }
	public void GetGreenPowerUp()
    {
		//Debug.Log("Pressed Green power up");

        distruptionFromGreenPowerUpActive = true;
        currentDistruptionFromOutsideFactors = greenPowerUpDistruptionMagnitude;
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
		hasPassedWaypointNumber++; // incremenet by one (FIXME: branching paths?)
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
		if (myWaypoint!=null) totalDistToNextWP = Vector3.Distance(transform.position, myWaypoint.trackPtForOffset(myTrackLaneOffset));
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
	
	#region OBSTACLE_ACTIVATION
	// NOTE(marvin): The trigger may occur for the same activator area multiple times. The code has been designed with
	// that in mind.
	void OnTriggerEnter(Collider other)
	{
		GameObject collidedGo = other.gameObject;
		bool enteredActivatorArea = collidedGo.TryGetComponent(out ActivatorArea area);
		if (enteredActivatorArea)
		{
			withinActivatorAreas.Add(area);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		GameObject collidedGo = other.gameObject;
		bool exitedActivatorArea = collidedGo.TryGetComponent(out ActivatorArea area);
		if (exitedActivatorArea)
		{
			withinActivatorAreas.Remove(area);
		}
	}
	#endregion
}
