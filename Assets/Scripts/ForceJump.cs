using UnityEngine;

public class ForceJump : MonoBehaviour
{
    public Vector3 Direction = Vector3.up;
    public float Force = 1.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 dir = Direction.normalized;
        float length = 8.0f;

        Gizmos.DrawLine(origin, origin + length * dir);
        Gizmos.DrawSphere(origin + length * dir, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.AddForce(Force * Direction, ForceMode.VelocityChange);

    }

}
