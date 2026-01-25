using System;
using UnityEngine;

public class PickUpItemScript : MonoBehaviour
{

    [SerializeField]
    private PickUpItemColors color;
    [SerializeField]
    [Tooltip("If this is set to true, when the object gets picked up the color will be random")]
    private bool isRandom = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isRandom) 
            {
                color = (PickUpItemColors)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PickUpItemColors)).Length - 1);
            }

            Debug.Log("Player picked up " + color.ToString() + " Item");

            //We might want a parent class for PlayerPowerUpManager and whatever class the AI will have for the manager so we can have one unified variable and method for both
            //If not AI will need different code here.
            other.TryGetComponent<PlayerPowerUpManager>(out PlayerPowerUpManager playerPowerUpManager);
            if (playerPowerUpManager != null)
            {
                playerPowerUpManager.AddPowerUp(color);
            }
        }
    }
}
