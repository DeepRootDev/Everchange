using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{

    // we aren't allowed to access layer names at editor startup
    // (only after awake or start)
    //[SerializeField] private LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayCastDistance = 1000;

    private bool   allowActivation = false;
    private ActivatorArea activatorArea;
    public SpeedrunStats myStats;

    void Awake()
    {
        if (layerMask == 0)
        {
            Debug.Log("Missing layerMask in PlayerPowerUpManager - assuming Obstacles layer.");
            layerMask = LayerMask.NameToLayer("Obstacles");
        }
    }

    void Update()
    {
        if (activatorArea != null)
        {
            if (Input.GetKeyDown(activatorArea.GetKeyType()) && allowActivation)
            {
                activatorArea.Toggle();
                if (myStats!=null) myStats.increaseTriggerCount();
            }

        }
    }

    private void FixedUpdate()
    {
        // FIXME: a tree or decoration might be in the way:
        // should we instead use something like Physics.SphereCast()
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayCastDistance,layerMask))
        {
            if(hit.transform.TryGetComponent(out ActivatorArea activatorArea))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                this.activatorArea  = activatorArea;
                allowActivation=true;
            }
            
        }
        else
        {
            allowActivation=false;
            if(activatorArea != null)
                activatorArea = null;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
          //  Debug.Log("Did not Hit");
        }
    }
}
