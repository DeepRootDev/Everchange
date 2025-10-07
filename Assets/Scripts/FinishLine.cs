using UnityEngine;

public class FinishLine : MonoBehaviour
{

    [Header("Drag the race end GUI object here:")]
    public GameObject activateThisWhenTriggered;

    [Header("Drag the SpeedrunStats GUI here:")]
    public SpeedrunStats saveResultsHere;

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
                // - stop the actual race
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
