using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("PLAYER MOVEMENT")]
    public bool debugMode = true; // for future use w gizmos etc
    public bool isGrounded = false;
    public bool isWallRunning = false;
    public bool isWallRunningLeft = false;
    public bool isWallRunningRight = false;
    public bool isDrifting = false;
    public bool isBoosting = false;
    public bool isGliding = false;
    public bool justJumped = false; // only true on the frame we started jumping
    public float currentSpeed = 0f; // public only to help debugging =)
    public float currentAltitude = 0f; // ground check result dist

    [Header("Movement Settings:")]
    public float runSpeed = 50f;
    public float boostSpeed = 100f;
    public float acceleration = 500f;
    public float gravityPower = -150f;
    public float jumpPower = 10f;
    public float groundedMaxAltitude = 7f; // dist from waist to ground (avg 4-6)
    
    [Header("Turning and Tilting:")]
    public float turnSpeed = 1f;
    public float corneringTiltDegreesMax = 15f;
    public float corneringTiltDegreesPerSec = 15f;
    //public float minTiltToBeDrifting = 14f;

    
    [Header("Wall Run:")]
    public float wallRunStickiness = 10f;
    public float wallrunMaxDistance = 3f;
    public float wallrunTiltDegreesMax = 65f;
    public float wallrunTiltDegreesPerSec = 65f;

    [Header("Wingsuit-style Gliding:")]
    public float glideTimeMax = 3f;
    public float glideTimeCur;
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

    [Header("Effects:")]
    public ParticleSystem groundedParticles;
    public ParticleSystem driftingParticles;
    public ParticleSystem wallrunLeftParticles;
    public ParticleSystem wallrunRightParticles;

    // FIXME: spherecast is broken? use raycast?
    /*
    [Header("FIXME: replace with a simple raycast")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    */

    public Transform debugTeleportPoint; // added to, for example, teleport to finish liine
    
    private Vector3 moveDirection;
    private Rigidbody rb;
    
    [Header("Drag the player's animated mesh here")]
    public Animator animator;
    private float globalPlaybackSpeed = 1.0f;    

    //WIP: Migrate to New Unity Input System
    private Vector2 moveInput;
    private bool jumpInputPerformed;
    private bool flyInputPerformed;
    private bool glideInputPerformed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // this won't work as it's on a child object
        if (!animator) animator = GetComponent<Animator>();
        glideTimeCur = glideTimeMax;

        if(PauseMenu.instance == null) {
            Debug.Log("note: PauseMenu.instance looks to be missing");
        }
    }

    void checkGrounded()
    {
        Ray rayDown = new Ray(transform.position, -transform.up);
        RaycastHit hitData;
        bool hitSomething = Physics.Raycast(rayDown, out hitData, 999f);
        if (hitSomething) {
            currentAltitude = hitData.distance;
            isGrounded = currentAltitude <= groundedMaxAltitude;
        } else { // there might be nothing below us
            isGrounded = false;
            currentAltitude = 999f;
        }

    }

    void Update()
    {
        if (PauseMenu.instance && PauseMenu.instance.isPaused)
        {
            return;
        }

        if(debugTeleportPoint != null)
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                transform.position = debugTeleportPoint.transform.position;
            }
        }

        // Ground Check - FIXME: doesn't work for arbitrary polygons without layer/tags
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        checkGrounded(); // uses a simple raycast instead

        // Get Input
        float horizontalInput = moveInput.x;

        // tilt as we turn sharper
        float myTiltNow = -1*horizontalInput*corneringTiltDegreesMax;

        // we drift if we're tilting a lot (due to a sharp turn held for a long time)
        isDrifting = isGrounded && // only drift when not in the air!!
            // can can't drift if we are moving straight
            horizontalInput != 0f &&
            //this decides we are drifting if we are tilting a lot
            // ((myTiltNow <= -minTiltToBeDrifting) || (myTiltNow >= minTiltToBeDrifting)
            glideInputPerformed; // right mouse button

        // float verticalInput = 1;//  always going forward! Input.GetAxis("Vertical");
        // normal platformer movement - strafe and fwd+back:
        // moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        
        // always running forward style:
        // turn like a car on left+right inputs
        Vector3 turnForce = new Vector3(0,horizontalInput*turnSpeed*Time.deltaTime,0);
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
                myTiltNow);

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
            (jumpInputPerformed)
            )
        {
            Jump();
            justJumped = true;
        } else {
            justJumped = false;
        }

        handleWallRun();

        // adjust particles depending on state
        updateParticleFX();

        // change animations depending on state
        if (animator) {
            // scale depending on rb velocity
            animator.speed = rb.linearVelocity.magnitude/runSpeed*globalPlaybackSpeed;
            // TODO: actually change animation
            if (isDrifting) animator.speed = 0f;
            if (isGliding) animator.speed = 0f;
        }
    }

    void updateParticleFX()
    {
        if (groundedParticles)
        {
            var em = groundedParticles.emission;
            em.rateOverTime = isGrounded ? 100 : 0;
        }
        if (driftingParticles)
        {
            var em = driftingParticles.emission;
            em.rateOverTime = isDrifting ? 1000 : 0;
        }
        if (wallrunLeftParticles)
        {
            var em = wallrunLeftParticles.emission;
            em.rateOverTime = isWallRunningLeft ? 1000 : 0;
        }
        if (wallrunRightParticles)
        {
            var em = wallrunRightParticles.emission;
            em.rateOverTime = isWallRunningRight ? 1000 : 0;
        }
    }

    void handleWallRun()
    {
        Ray rayLeft = new Ray(transform.position, -transform.right);
        Ray rayRight = new Ray(transform.position, transform.right);
        RaycastHit hitData;
        // left arm
        isWallRunning = Physics.Raycast(rayLeft, out hitData, wallrunMaxDistance);
        isWallRunningLeft = isWallRunning;
        // right arm
        if (!isWallRunning) 
        { 
            isWallRunning = Physics.Raycast(rayRight, out hitData, wallrunMaxDistance);
            isWallRunningRight = isWallRunning;
        }
        // either
        if (isWallRunning)
        {
            float hitAngle = Vector3.Angle(hitData.normal, transform.up) - 90f;
            // handy but spammy debug log commented out
            // Debug.Log("WALL RUNNING on " + hitData.collider.name + " at point: " + hitData.point + " angle:"+hitAngle+" dist:"+hitData.distance);
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

    // FIXME: use new input system
    void FixedUpdate()
    {
        if (PauseMenu.instance && PauseMenu.instance.isPaused)
        {
            return;
        }
        float targetSpeed = 0f; // default to stand still when no input

        // move forward
        if (moveInput.y > 0) {
            targetSpeed = runSpeed;
        }

        // make local state match the global variable
        isBoosting = BoostAbility.isBoosting;
        if (isBoosting) targetSpeed = boostSpeed;

        isGliding = (!isGrounded && flyInputPerformed);
        if (isGliding) { 
            // glide also has a speed boost
            targetSpeed = glideSpeed;

            glideTimeCur -= Time.deltaTime;
            glideTimeCur = Mathf.Clamp(glideTimeCur,0,glideTimeMax);

            if (glideTimeCur <= 0.1f)
            {
                isGliding = false;
            }
        }
        else
        {
            glideTimeCur += Time.deltaTime;
            glideTimeCur = Mathf.Clamp(glideTimeCur, 0, glideTimeMax);
        }

        // Apply Movement Force
        if (targetSpeed>0) {
            rb.AddForce(moveDirection * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
            // Limit horizontal velocity to prevent excessive speed
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > targetSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * targetSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
        currentSpeed = rb.linearVelocity.magnitude; // now test the result for debugging

        // add gravity unless we're touching ground or wall, or gliding
        if (!isGrounded && !isWallRunning && !isGliding) rb.AddForce(new Vector3(0, gravityPower, 0), ForceMode.Acceleration);

        // fly up or down while gliding
        if (isGliding)
        {
            
            if (moveInput.y <= 0)
            {
                rb.AddForce(new Vector3(0, gravityPower, 0), ForceMode.Acceleration);
            }

            if (jumpInputPerformed)
            {
                rb.AddForce(new Vector3(0, -gravityPower, 0), ForceMode.Acceleration);
            }
        }
        

    }

    void Jump()
    {
        Debug.Log("JUMP! isGrounded="+isGrounded); // should always be true!!
        // Reset vertical velocity before jumping to ensure consistent jump height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    #region InputSystem

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
       //Debug.Log($"move inp: {moveInput}");
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        jumpInputPerformed = ctx.performed;
    }

    public void onFly(InputAction.CallbackContext ctx)
    {
        flyInputPerformed = ctx.performed;
    }

    public void onGlide(InputAction.CallbackContext ctx)
    {
        glideInputPerformed = ctx.performed;
    }

    #endregion

}
