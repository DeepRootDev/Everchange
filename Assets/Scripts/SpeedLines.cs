using UnityEngine;

public class SpeedLines : MonoBehaviour
{
    public float particlesPerSecWhenBoosting = 100f;
    public float particlesPerSecWhenNotBoosting = 0f;
    private ParticleSystem.EmissionModule emission;
    public PlayerMovement myPlayerMovement;
    public bool isCurrentlyOn = false; // only used for debug!
    
    void Start()
    {
        emission = GetComponent<ParticleSystem>().emission;
    }

    void Update()
    {
        if (BoostAbility.isBoosting||(myPlayerMovement&&myPlayerMovement.isBoosting)) {
            isCurrentlyOn = true;
            emission.rateOverTime = particlesPerSecWhenBoosting;
        } else {
            isCurrentlyOn = false;
            emission.rateOverTime = particlesPerSecWhenNotBoosting;
        }
    }
}
