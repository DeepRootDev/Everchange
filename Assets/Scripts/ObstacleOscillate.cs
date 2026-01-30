using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObstacleOscillate : Obstacle
{
    [SerializeField] private float phaseShiftRate = -30f;
    float rateAdj = 0.02f;
    public Transform pos1;
    public Transform pos2;
    private float phase = 0.0f;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (IsActive)
        {
            phase += phaseShiftRate * Time.deltaTime;
            float phasePerc = Mathf.Clamp01(Mathf.Cos(phase * rateAdj) * 0.5f + 0.5f);
            transform.position = Vector3.Lerp(pos1.position, pos2.position, phasePerc);

            if ((phasePerc > 0.99f || phasePerc < 0.01) && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
