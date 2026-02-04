using UnityEngine;
using UnityEngine.InputSystem;

public class SlowChase : MonoBehaviour
{
    [Header("Normal Camera Position")]
    public Transform chaseTarget;
    public Transform lookPoint;

    [Header("Camera Pos When Drifting")]
    public Transform driftChaseTarget;
    public float driftExtraCamRotation = 15f;


    [Header("Game Intro Flyby")]
    public Transform introStartAt;
    public Transform introEndAt;
    public Transform introLookAt;
    public float introLengthInSeconds = 3f; // set to zero for no intro
    private float introTimeleft;

    [Header("Mouse Look")]
    public bool mouseLookEnabled = true;
    public bool hideMouseCursor = true;
    public float mouseLookSpeedX = 1.3f;
    public float mouseLookSpeedY = 1.3f;
    public float mouseLookVertRange = 65f; // degrees + or -

    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private float posHalfLife = 0.05f;
    private float rotHalfLife = 0.10f;

    public PlayerMovement myPlayerMovement;

    private InputAction lookAction;


    void Start()
    {
        introTimeleft = introLengthInSeconds;

        if (hideMouseCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        lookAction = InputSystem.actions.FindAction("Player/Look");
    }

    private void LateUpdate()
    {
        // messy because I need it smooth for camera movement (can't do FixedUpdate)
        // but since lerp and slerp aren't linear just multiplying by time.deltaTime would break things
        // more thorough explanation of why this is messy math: https://medium.com/@tglaiel/how-to-make-your-game-run-at-60fps-24c61210fe75
        float posT = Mathf.Clamp01(1f - Mathf.Pow(0.5f, Time.deltaTime / Mathf.Max(Mathf.Epsilon, posHalfLife)));
        float rotT = Mathf.Clamp01(1f - Mathf.Pow(0.5f, Time.deltaTime / Mathf.Max(Mathf.Epsilon, rotHalfLife)));

        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(lookPoint.position - transform.position), rotT);

        if (mouseLookEnabled)
        {
            Vector2 look = lookAction.ReadValue<Vector2>();
            rotationX += look.x * mouseLookSpeedX;
            rotationY += -1 * look.y * mouseLookSpeedY;
            rotationY = Mathf.Clamp(rotationY, -mouseLookVertRange, mouseLookVertRange);

            // adds just this frame's movement: works great
            // howeverwe lerp back once we stop moving the mouse
            transform.Rotate(rotationY, rotationX, 0f);
            rotationX = 0f;
            rotationY = 0f;

            // alternate way - not quite working
            // add the rotational offset fresh every frame without resetting
            // Quaternion QuatOffsetX = Quaternion.AngleAxis(rotationX, Vector3.up);
            // Quaternion QuatOffsetY = Quaternion.AngleAxis(rotationY, Vector3.left);
            // transform.rotation *= QuatOffsetX;
            // transform.rotation *= QuatOffsetY;

        }

        if (introTimeleft > 0f && introStartAt != null) // intro flyby time?
        {
            // do a face closeup camera effect during the 3..2..1
            introTimeleft -= Time.deltaTime;
            transform.position = Vector3.Slerp(introStartAt.position, introEndAt.position, 1f-(introTimeleft/introLengthInSeconds));
            transform.rotation = Quaternion.LookRotation(introLookAt.position - transform.position);
        } else if (myPlayerMovement&&myPlayerMovement.isDrifting&&driftChaseTarget) {
            // move back a bit during drifting
            transform.position = Vector3.Slerp(transform.position, driftChaseTarget.position, posT);
            // and peer around the corner? which side?
            // transform.Rotate(driftExtraCamRotation, 0f, 0f);
        } else {    
            // follow the normal gameplay chase camera target
            transform.position = Vector3.Slerp(transform.position, chaseTarget.position, posT);
        }
    }
}
