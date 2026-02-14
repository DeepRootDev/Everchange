using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

    public static event Action OnCameraAnimationComplete;



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

        Vector3 desiredPos;
        Quaternion desiredRot;

        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(lookPoint.position - transform.position), rotT);

        transform.position = Vector3.Slerp(transform.position, chaseTarget.position, posT);

        if (introTimeleft > 0f && introStartAt != null) // intro flyby time?
        {
            // do a face closeup camera effect during the 3..2..1
            introTimeleft -= Time.deltaTime;
            transform.position = Vector3.Lerp(introStartAt.position, introEndAt.position, 1f-(introTimeleft/introLengthInSeconds));
            transform.rotation = Quaternion.LookRotation(introLookAt.position - transform.position);            
            return;
        } else if (myPlayerMovement&&myPlayerMovement.isDrifting&&driftChaseTarget) {
            // move back a bit during drifting
            desiredPos = driftChaseTarget.position;
            // and peer around the corner? which side?
            // transform.Rotate(driftExtraCamRotation, 0f, 0f);
        } else {    
            // follow the normal gameplay chase camera target
            desiredPos = chaseTarget.position;
            OnCameraAnimationComplete?.Invoke();
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, posT);

        if (introTimeleft > 0f && introStartAt != null)
            desiredRot = Quaternion.LookRotation(introLookAt.position - transform.position);
        else
            desiredRot = Quaternion.LookRotation(lookPoint.position - transform.position);

        if (mouseLookEnabled)
        {
            Vector2 look = lookAction.ReadValue<Vector2>();

            rotationX += look.x * mouseLookSpeedX;
            rotationY += -look.y * mouseLookSpeedY;
            rotationY = Mathf.Clamp(rotationY, -mouseLookVertRange, mouseLookVertRange);

            Quaternion yawQ = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yawed = yawQ * Quaternion.LookRotation(lookPoint.position - transform.position);

            Vector3 rightAxis = yawed * Vector3.right;
            Quaternion pitchQ = Quaternion.AngleAxis(rotationY, rightAxis);

            Quaternion desired = pitchQ * yawed;

            transform.rotation = Quaternion.Slerp(transform.rotation, desired, rotT);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(lookPoint.position - transform.position),rotT);
        }
    }
}
