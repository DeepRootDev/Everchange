using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public float runningSoundVolume = 1f;
    public float boostSoundVolume = 1f;

    private WaypointDrive myWPD;
    //private BoostAbility myBoost;
    private AudioSource runningSound;
    private AudioSource boostSound;
    private bool wasBoostingLastFrame = false;

    void Start()
    {
        myWPD = transform.parent.GetComponent<WaypointDrive>();
        //myBoost = GetComponent<BoostAbility>();
        runningSound = GetComponents<AudioSource>()[0];
        boostSound = GetComponents<AudioSource>()[1];
    }
    
    void Update()
    {
        // don't crash if any of these are null
        if (!myWPD) return;
        //if (!myBoost) return;
        if (!runningSound) return;
        if (!boostSound) return;

        if (myWPD.inAir)
        {
            runningSound.volume = 0;
        }
        else
        {
            runningSound.volume = runningSoundVolume;
        }


        if (BoostAbility.isBoosting)
        {
            if (!wasBoostingLastFrame)
            {
                boostSound.Play();
            }
        }
        wasBoostingLastFrame = BoostAbility.isBoosting;

    }
}
