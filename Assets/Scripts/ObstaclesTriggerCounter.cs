using System;
using UnityEngine;

public class ObstaclesTriggerCounter : MonoBehaviour
{
    public static event Action<int,int> OnTriggerValueChanged;

    private int redTriggerCount;
    private int blueTriggerCount;

    private void Start()
    {
        PlayerPowerUpManager.OnObstacleTriggered += PlayerPowerUpManager_OnObstacleTriggered;
    }

    private void OnDestroy()
    {
        PlayerPowerUpManager.OnObstacleTriggered -= PlayerPowerUpManager_OnObstacleTriggered;
    }

    private void PlayerPowerUpManager_OnObstacleTriggered(PickUpItemColors obj)
    {
        switch (obj)
        {
            case PickUpItemColors.red:
                redTriggerCount++;
                break;
            case PickUpItemColors.blue:
                blueTriggerCount++;
                break;
            case PickUpItemColors.green:
                break;
            default:
                break;
        }

        OnTriggerValueChanged?.Invoke(redTriggerCount,blueTriggerCount);
    }
}
