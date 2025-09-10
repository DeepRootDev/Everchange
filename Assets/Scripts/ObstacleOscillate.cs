using UnityEngine;

public class ObstacleOscillate : MonoBehaviour
{
    float phaseShiftRate = -30f;
    float rateAdj = 0.02f;
    public Transform pos1;
    public Transform pos2;
    private float phase = 0.0f;

    void Update()
    {
        phase += phaseShiftRate * Time.deltaTime;
        float phasePerc = Mathf.Clamp01(Mathf.Cos(phase*rateAdj)*0.5f+0.5f); 
        transform.position = Vector3.Lerp(pos1.position, pos2.position, phasePerc);
    }
}
