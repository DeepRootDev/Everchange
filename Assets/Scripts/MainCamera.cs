using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float boostFOV;
    private Camera camera;
    private float normalFOV;

    private void Awake()
    {
        camera = GetComponent<Camera>();

        normalFOV = camera.fieldOfView;
    }

    private void Update()
    {
        if (WaypointDrive.isBoosting)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, boostFOV, Time.deltaTime * WaypointDrive.toReachBoostSpeed);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFOV, Time.deltaTime * WaypointDrive.toReachBoostSpeed);
        }

    }
}
