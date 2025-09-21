using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
    [SerializeField] private float rayCastDistance = 1000;

    private bool   allowAcitvation = false;
    private ActivatorArea activatorArea;

    void Update()
    {
        if (activatorArea != null)
        {
            if (Input.GetKeyDown(activatorArea.GetKeyType()) && allowAcitvation)
            {
                activatorArea.Toggle();
            }

        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayCastDistance,layerMask))
        {
            if(hit.transform.TryGetComponent(out ActivatorArea activatorArea))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                this.activatorArea  = activatorArea;
                allowAcitvation=true;
            }
            
        }
        else
        {
            allowAcitvation=false;
            if(activatorArea != null)
                activatorArea = null;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
          //  Debug.Log("Did not Hit");
        }
    }
}
