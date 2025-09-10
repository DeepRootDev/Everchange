using UnityEngine;

public class SlowChase : MonoBehaviour
{
    public Transform chaseTarget;
    public Transform lookPoint;

    void LateUpdate()
    {
        transform.position = chaseTarget.position;
        transform.LookAt(lookPoint);
    }
}
