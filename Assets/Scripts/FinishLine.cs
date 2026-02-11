using UnityEngine;
using UnityEngine.InputSystem;

public class FinishLine : MonoBehaviour
{

    [Header("Drag the race end GUI object here:")]
    public GameObject activateThisWhenTriggered;

    [Header("Drag the SpeedrunStats GUI here:")]
    public SpeedrunStats saveResultsHere;

    // why is this here?
    // public InputActionAsset inputActions;
    // private InputActionMap playerActionMap;

    void Start()
    {
        // buggy:
        // playerActionMap = inputActions.FindActionMap("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + " hit the Finish Line!");
            if (activateThisWhenTriggered)
            {
                // show the race finish screen
                activateThisWhenTriggered.SetActive(true);
                // TODO:
                Debug.Log("Pausing the game!");
                Time.timeScale = 0f;
                // playerActionMap.Disable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // - save the results to speedrunstats db
                // - wait for input
            }
        }
    }

    private void OnDrawGizmos()
    {
        // a black box with white outline for debug viewing and scaling
        Color rgba = Color.black;
        rgba.a = 0.666f;
        Gizmos.color = rgba;
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

}
