using UnityEngine;

public class ObstacleSpin : MonoBehaviour
{
    float speedAndDir = -30f;
    Quaternion startRot;
    private void Start()
    {
        startRot = transform.rotation;
    }
    void Update()
    {
        transform.rotation = startRot * Quaternion.AngleAxis(Time.timeSinceLevelLoad * speedAndDir, Vector3.up);        
    }
}
