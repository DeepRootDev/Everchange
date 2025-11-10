using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public float particlesPerSecWhenBoosting = 100f;
    public float particlesPerSecWhenNotBoosting = 0f;
    private ParticleSystem.EmissionModule emission;
    
    void Start()
    {
        emission = GetComponent<ParticleSystem>().emission;
    }

    void Update()
    {
        emission.rateOverTime = (BoostAbility.isBoosting ? particlesPerSecWhenBoosting : particlesPerSecWhenNotBoosting);
    }
}
