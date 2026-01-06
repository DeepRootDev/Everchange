using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float boostFOV;
    [SerializeField] private float grindFOV;
    // renamed to "mycamera" to avoid unity compiler warning
    // why does this happen?
    // because unity already uses it so the names collide
    private Camera mycamera; 
    private float normalFOV;

    public PlayerMovement myPlayerMovement;

    private void Awake()
    {
        mycamera = GetComponent<Camera>();
        normalFOV = mycamera.fieldOfView;
    }

    private void Update()
    {
        // bosting camera angle
        if (BoostAbility.isBoosting || 
            (myPlayerMovement&&myPlayerMovement.isBoosting))
        {
            mycamera.fieldOfView = Mathf.Lerp(mycamera.fieldOfView, boostFOV, Time.deltaTime * BoostAbility.toReachBoostSpeed);
        }
        else if (myPlayerMovement&&myPlayerMovement.isGrinding)
        {
            mycamera.fieldOfView = Mathf.Lerp(mycamera.fieldOfView, grindFOV, Time.deltaTime * BoostAbility.toReachBoostSpeed);
        }
        else // normal camera angle
        {
            mycamera.fieldOfView = Mathf.Lerp(mycamera.fieldOfView, normalFOV, Time.deltaTime * BoostAbility.toReachBoostSpeed);
        }

    }
}
