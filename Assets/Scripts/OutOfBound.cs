using UnityEngine;

public class OutOfBound : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        other.transform.position = PlayerCollisionManager.playerPositionOnExit + new Vector3(0,0,0);
    }
}
