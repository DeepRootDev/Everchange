using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{

    // we aren't allowed to access layer names at editor startup
    // (only after awake or start)
    //[SerializeField] private LayerMask layerMask = LayerMask.NameToLayer("Obstacles");
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayCastDistance = 1000;

    [SerializeField] public Dictionary<PickUpItemColors, bool> currentPickedUpItems;
    [SerializeField] private int maxNumberOfPickUpItems = 2;

    private bool   allowActivation = false;
    private ActivatorArea activatorArea;
    public SpeedrunStats myStats;

    private GameObject[] EnemyArray;
    [SerializeField]
    private KeyCode greenPowerUpKeyCode = KeyCode.C;

    

    void Awake()
    {
        if (layerMask == 0)
        {
            Debug.Log("Missing layerMask in PlayerPowerUpManager - assuming Obstacles layer.");
            layerMask = LayerMask.NameToLayer("Obstacles");
        }


        //initiliaze pickupitems as none picked up at the beginning
        currentPickedUpItems = new Dictionary<PickUpItemColors, bool>();
        foreach (PickUpItemColors color in Enum.GetValues(typeof(PickUpItemColors)))
        {
            currentPickedUpItems[color] = false;
        }
    }
    void Start()
    {
        EnemyArray = GameObject.FindGameObjectsWithTag("Player").Where(x => !GameObject.ReferenceEquals(x, gameObject)).ToArray();
    }

    void Update()
    {
        if (activatorArea != null)
        {
            if (Input.GetKeyDown(activatorArea.GetKeyType()) && allowActivation && CheckAreaColorForPowerUp())
            {
                Debug.Log("Trying to activate " + activatorArea.GetAreaColor() + " Zone");
                activatorArea.Toggle();
                currentPickedUpItems[activatorArea.GetAreaColor()] = false; //since we used it once now it is false.
                if (myStats!=null) myStats.increaseTriggerCount();
            }

        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(greenPowerUpKeyCode))
        {
            UseGreenPowerUp();
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
                // removed this debug log because it ran every frame, hiding other debug text:
                // Debug.Log("Did Hit");
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

    public void UseGreenPowerUp()
    {
        GameObject enemy = GetClosestEnemy();
        enemy.GetComponent<WaypointDrive>().distruptionFromGreenPowerUpActive = true;
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
        if (currentPickedUpItems.Count(x => x.Value) <= maxNumberOfPickUpItems)
        {
            currentPickedUpItems[color] = true;
        }
    }

    private bool CheckAreaColorForPowerUp()
    {
        if (activatorArea != null)
        {
            PickUpItemColors areaColor = activatorArea.GetAreaColor();

            if (currentPickedUpItems[areaColor])
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
}
