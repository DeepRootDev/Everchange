using UnityEngine;

public class OutOfBound : MonoBehaviour
{
    public float respawnAddedHeight = 100f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) 
        {
            Debug.Log("Player fell out of bounds! Respawning near last collision...");
            other.transform.position = PlayerCollisionManager.playerPositionOnExit 
                + new Vector3(0,respawnAddedHeight,0); // respawn a bit higher so you are less likely to get stuck
        }
    }
}
