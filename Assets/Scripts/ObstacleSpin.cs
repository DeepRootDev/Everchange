using UnityEngine;

public class ObstacleSpin : Obstacle 
{
    float speedAndDir = -30f;
    public Vector3 rotAxis = Vector3.up;
    Quaternion startRot;


    private void Start()
    {
        startRot = transform.rotation;
    }
    void Update()
    {
        if(IsActive)
        transform.rotation = startRot * Quaternion.AngleAxis(Time.timeSinceLevelLoad * speedAndDir, rotAxis);        
    }


}
