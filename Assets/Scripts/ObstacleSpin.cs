using UnityEngine;

public class ObstacleSpin : Obstacle 
{
    [SerializeField] private float speed = -30f;
    [SerializeField] private Vector3 rotAxis = Vector3.up;
 
    void Update()
    {
        if(IsActive)
        transform.Rotate(rotAxis * Time.deltaTime * speed, Space.Self);
    }
}
