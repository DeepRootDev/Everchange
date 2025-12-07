using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float boostFOV;
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
        if (BoostAbility.isBoosting || 
            (myPlayerMovement&&myPlayerMovement.isBoosting))
        {
            mycamera.fieldOfView = Mathf.Lerp(mycamera.fieldOfView, boostFOV, Time.deltaTime * BoostAbility.toReachBoostSpeed);
        }
        else
        {
            mycamera.fieldOfView = Mathf.Lerp(mycamera.fieldOfView, normalFOV, Time.deltaTime * BoostAbility.toReachBoostSpeed);
        }

    }
}
