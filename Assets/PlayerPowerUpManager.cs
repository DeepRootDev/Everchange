using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class PlayerPowerUpManager : MonoBehaviour
{

    // we aren't allowed to access layer names at editor startup
    // (only after awake or start)
    //[SerializeField] private LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayCastDistance = 1000;
    [SerializeField] private bool powerTest  = false;
    [SerializeField] public List<PowerUpItemScriptableObject> currentPickedUpItems;
    [SerializeField] private int maxNumberOfPickUpItems = 2;
    private bool allowActivationInsideActivatorArea = false;

    private bool   allowActivationUsingRayCast = false;
    private ActivatorArea activatorArea;
    public SpeedrunStats myStats;

    private GameObject[] EnemyArray;
    [SerializeField]
    private KeyCode greenPowerUpKeyCode = KeyCode.C;
    private InputAction greenPowerUpAction;
    private InputAction activateAreaActionRed;
    private InputAction activateAreaActionBlue;



    void Awake()
    {
        activateAreaActionRed = new InputAction(name: "activateAreaActionRed", type: InputActionType.Button);
        activateAreaActionBlue = new InputAction(name: "activateAreaActionBlue", type: InputActionType.Button);
        activateAreaActionRed.AddBinding("<Keyboard>/q");
        activateAreaActionBlue.AddBinding("<Keyboard>/e");
        activateAreaActionRed.Enable();
        activateAreaActionBlue.Enable();

        activateAreaActionRed.performed += ActivateAreaActionRed_performed;
        activateAreaActionBlue.performed += ActivateAreaActionBlue_performed;


        if (layerMask == 0)
        {
            Debug.Log("Missing layerMask in PlayerPowerUpManager - assuming Obstacles layer.");
            layerMask = LayerMask.NameToLayer("Obstacles");
        }


        //initiliaze pickupitems as none picked up at the beginning
        foreach (PowerUpItemScriptableObject powerUpItem in currentPickedUpItems)
        {
            powerUpItem.NumberOfUsesLeft = 0;
        }

        greenPowerUpAction = new InputAction(name: "GreenPowerUpAction", type: InputActionType.Button);
        greenPowerUpAction.AddBinding("<Keyboard>/" + greenPowerUpKeyCode.ToString());
        greenPowerUpAction.performed += onGreenPowerUp;
        greenPowerUpAction.Enable();
    }

    private void ActivateAreaActionRed_performed(InputAction.CallbackContext obj)
    {
        TryActivate(PickUpItemColors.red);
    }

    private void ActivateAreaActionBlue_performed(InputAction.CallbackContext obj)
    {
        TryActivate(PickUpItemColors.blue);
    }

    private void TryActivate(PickUpItemColors inputColor)
    {
        if (activatorArea == null)
            return;

        if (!allowActivationUsingRayCast && !allowActivationInsideActivatorArea)
            return;

        if (activatorArea.GetAreaColor() != inputColor)
            return;

        if (!CheckAreaColorForPowerUp())
            return;

        activatorArea.Toggle();

        var item = currentPickedUpItems
            .FirstOrDefault(x => x.Color == inputColor);

        if (item != null)
            item.NumberOfUsesLeft--;

        if (myStats != null)
            myStats.increaseTriggerCount();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ActivatorArea>(out ActivatorArea area))
        {
            if (activatorArea == area)
            {
                allowActivationInsideActivatorArea = false;
                activatorArea = null;
            }
        }
    }


    void Start()
    {
        EnemyArray = GameObject.FindGameObjectsWithTag("Player").Where(x => !GameObject.ReferenceEquals(x, gameObject)).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ActivatorArea>(out ActivatorArea area))
        {            
            

            allowActivationInsideActivatorArea = true;
            activatorArea = area;
            
        }
    }


   



    private void FixedUpdate()
    {
        // FIXME: a tree or decoration might be in the way:
        // should we instead use something like Physics.SphereCast()
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayCastDistance,layerMask))
        {
            if(hit.transform.TryGetComponent<ActivatorArea>(out ActivatorArea area))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                // removed this debug log because it ran every frame, hiding other debug text:
                // Debug.Log("Did Hit");
                if(area!=null)
                {
                    allowActivationUsingRayCast=true;
                    activatorArea = area;
                }
                else
                {
                    allowActivationUsingRayCast=false;
                }
            }
            
        }
        else
        {
            allowActivationUsingRayCast=false;
           
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
          //  Debug.Log("Did not Hit");
        }
    }

    public void UseGreenPowerUp()
    {
        PowerUpItemScriptableObject greenPowerUp = currentPickedUpItems.FirstOrDefault(x => x.Color == PickUpItemColors.green);
        if (greenPowerUp?.NumberOfUsesLeft > 0)
        {
            GameObject enemy = GetClosestEnemy();
            if (enemy != null)
            {
                enemy.GetComponent<WaypointDrive>().GetGreenPowerUp();
            }
            
        }
        
    }

    private GameObject GetClosestEnemy()
    {
        GameObject closest = null;
        float minDistance = float.MaxValue;
        Vector3 currentPos = transform.position;

        foreach (var enemy in EnemyArray)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(currentPos, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    public void AddPowerUp(PickUpItemColors color)
    {
        if (currentPickedUpItems.Sum(x => x.NumberOfUsesLeft) <= maxNumberOfPickUpItems)
        {
            currentPickedUpItems.FirstOrDefault(x => x.Color == color).NumberOfUsesLeft += 1;
        }
    }

    private bool CheckAreaColorForPowerUp()
    {
        if (powerTest) return true;

        if (activatorArea != null)
        {
            PickUpItemColors areaColor = activatorArea.GetAreaColor();

            if (currentPickedUpItems.FirstOrDefault(x => x.Color == areaColor).NumberOfUsesLeft > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #region Input Action Handling
    public void onGreenPowerUp(InputAction.CallbackContext ctx)
    {
        if (greenPowerUpAction.WasPerformedThisFrame())
        {
            UseGreenPowerUp();
        }
    }
    #endregion

    void OnDestroy()
    {
        greenPowerUpAction.Disable();
        greenPowerUpAction.Dispose();
    }
}
