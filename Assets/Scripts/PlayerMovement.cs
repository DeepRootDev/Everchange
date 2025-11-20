using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float currentSpeed = 0f; // public only to help debugging =)
    public float minSpeed = 50f;
    public float maxSpeed = 100f;
    public float turnSpeed = 1f;
    public float acceleration = 500f;
    public float gravityPower = -150f;
    public float jumpPower = 10f;
    public float wallRunStickiness = 10f;
    public float glideTime = 3f;
    public float boostPower = 50f;
    public float gravity = 10f;
    public float driftFriction = 1f;
 public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ground Check
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

        // Jump Input
        if (Input.GetButtonDown("Jump") || // hmm not working
            Input.GetKey(KeyCode.Space) // oldschool
            && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = minSpeed;
        // hmm this is never true...
        if (BoostAbility.isBoosting) targetSpeed = maxSpeed;
        // ok fake it here
        if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = maxSpeed;

        // Apply Movement Force
        rb.AddForce(moveDirection * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Limit horizontal velocity to prevent excessive speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        currentSpeed = flatVel.magnitude;
        if (currentSpeed > targetSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * targetSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        // add in our fake gravity
        if (!isGrounded) rb.AddForce(new Vector3(0, gravityPower, 0), ForceMode.Acceleration);        

    }

    void Jump()
    {
        // Reset vertical velocity before jumping to ensure consistent jump height
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}
