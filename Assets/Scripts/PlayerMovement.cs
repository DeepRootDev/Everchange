using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("PLAYER MOVEMENT")]
    public bool debugMode = true; // for future use w gizmos etc
    public bool isGrounded = false;
    public bool isWallRunning = false;
    public bool isGrinding = false;
    public bool isBoosting = false;
    public bool isGliding = false;
    public float currentSpeed = 0f; // public only to help debugging =)

    [Header("Movement Settings:")]
    public float minSpeed = 50f;
    public float maxSpeed = 100f;
    public float acceleration = 500f;
    public float gravityPower = -150f;
    public float jumpPower = 10f;
    
    [Header("Turning and Tilting:")]
    public float turnSpeed = 1f;
    public float corneringTiltDegreesMax = 15f;
    public float corneringTiltDegreesPerSec = 15f;
    
    [Header("Wall Run:")]
    public float wallRunStickiness = 10f;
    public float wallrunMaxDistance = 3f;
    public float wallrunTiltDegreesMax = 65f;
    public float wallrunTiltDegreesPerSec = 65f;
    [Header("Wingsuit-style Gliding:")]
    public float glideTime = 3f;
    public float glideSpeed = 75f;
    public float glideLeanAngle = 90;
    public float glideLeanSpeed = 1;

    [Header("Speed Boost:")]
    public float boostPower = 50f;
    public float boostTimespan = 3f;
    [Header("Drifting Turns:")]
    public float driftTriggerSpeed = 1f;
    public float driftTriggerAngle = 15f;
    public float driftFriction = 1f;
    public float driftTiltDegreesMax = 65f;
    public float driftTiltDegreesPerSec = 65f;

    // a gameobject at pos 0,0,0 rot 0,0,0 and scale 1,1,1
    // that holds the scaled and rotated player mesh
    // this way we can tilt it without tilting the rigidbody etc
    [Header("Something we can tilt without affecting physics")]
    public Transform thePlayerVisuals; 

    // FIXME: spherecast is broken? use raycast?
    [Header("FIXME: replace with a simple raycast")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    
    private Vector3 moveDirection;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ground Check - FIXME: doesn't work for arbitrary polygons without layer/tags
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Get Input
        float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput = 1;//  always going forward! Input.GetAxis("Vertical");
        // normal platformer movement - strafe and fwd+back:
        // moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        
        // always running forward style:
        // turn like a car on left+right inputs
        Vector3 turnForce = new Vector3(0,horizontalInput*turnSpeed,0);
        transform.Rotate(turnForce);

        moveDirection = transform.forward;
        moveDirection.Normalize(); // Normalize to prevent faster diagonal movement

        // tilt player mesh a little when we are cornering
        if (thePlayerVisuals)
        {
            // tilt the player left and right as we turn
            thePlayerVisuals.transform.localRotation = Quaternion.Euler(
                thePlayerVisuals.transform.localRotation.x,
                thePlayerVisuals.transform.localRotation.y,
                -1*horizontalInput*corneringTiltDegreesMax);

            // tilt the player forwards to glide like a wingsuit
            if (isGliding) {
                // counteract gravity?
                // rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
                // lean forward
                thePlayerVisuals.transform.localRotation = Quaternion.Euler(
                    glideLeanAngle, // FIXME: tween using glideLeanSpeed
                    thePlayerVisuals.transform.localRotation.y,
                    thePlayerVisuals.transform.localRotation.z
                    );
            }
        }

        // Jump Input
        if (isGrounded &&
            Input.GetButtonDown("Jump") || // hmm not working
            Input.GetKey(KeyCode.Space)) // oldschool
        {
            Jump();
        }

        handleWallRun();
    }

    void handleWallRun()
    {
        Ray rayLeft = new Ray(transform.position, -transform.right);
        Ray rayRight = new Ray(transform.position, transform.right);
        RaycastHit hitData;
        isWallRunning = Physics.Raycast(rayLeft, out hitData, wallrunMaxDistance);
        if (!isWallRunning) isWallRunning = Physics.Raycast(rayRight, out hitData, wallrunMaxDistance);
        if (isWallRunning)
        {
            float hitAngle = Vector3.Angle(hitData.normal, transform.up) - 90f;
            Debug.Log("WALL RUNNING on " + hitData.collider.name + " at point: " + hitData.point + " angle:"+hitAngle+" dist:"+hitData.distance);
            // while wall running, there's no gravity applied in update function
        }        

        // tilt the player if required
        // FIXME: gimbal lock wierdness
        /*
        if (thePlayerMesh)
        {
            Quaternion target = transform.rotation; //Quaternion.Euler(-90f, 0f, 0f);
            target.x = -90f; // fwd/back tilt based on art
            if (isWallRunning) {
                target.z = wallrunTiltDegreesMax;
            }
            thePlayerMesh.transform.rotation = target; //Quaternion.Slerp(thePlayerMesh.transform.rotation, target,  Time.deltaTime * wallrunTiltDegreesPerSec);
        }
        */

    }

    void FixedUpdate()
    {
        float targetSpeed = minSpeed;
        // hmm this is never true...
        if (BoostAbility.isBoosting) targetSpeed = maxSpeed;
        // ok fake it here
        if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = maxSpeed;
        // but tell the world
        isBoosting = (targetSpeed == maxSpeed);

        // FIXME:use new input system
        isGliding = (!isGrounded && Input.GetMouseButton(1));
        if (isGliding) { 
            // glide also has a speed boost
            targetSpeed = glideSpeed;
        }

        // Apply Movement Force
        rb.AddForce(moveDirection * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Limit horizontal velocity to prevent excessive speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > targetSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * targetSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        currentSpeed = rb.linearVelocity.magnitude; // now test the result for debugging

        // add gravity unless we're touching ground or wall, or gliding
        if (!isGrounded && !isWallRunning && !isGliding) rb.AddForce(new Vector3(0, gravityPower, 0), ForceMode.Acceleration);        

    }

    void Jump()
    {
        // Reset vertical velocity before jumping to ensure consistent jump height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}
