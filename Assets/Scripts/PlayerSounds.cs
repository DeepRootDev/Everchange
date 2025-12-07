using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public PlayerMovement myPlayerMovement;
    public float runningSoundVolume = 1f;
    public float boostSoundVolume = 1f;
    public float wallrunSoundVolume = 1f;
    public float grindingSoundVolume = 1f;

    // the waypoint way - FIXME:
    // private WaypointDrive myWPD;
    // private BoostAbility myBoost;
    private AudioSource runningSound;
    private AudioSource boostSound;
    private AudioSource wallrunSound;
    private AudioSource grindingSound;
    private bool wasBoostingLastFrame = false;

    // only used for debug vis in editor
    public float currentRunVol = 0f;
    public float currentBoostVol = 0f;
    public float currentWallrunVol = 0f;
    public float currentGrindingVol = 0f;

    // not Awake - this is fired twice?!
    void Start()
    {
        //myWPD = transform.parent.GetComponent<WaypointDrive>();
        //myBoost = GetComponent<BoostAbility>();
       
        Debug.Log("We need 4 audiosources and have "+GetComponents<AudioSource>().Length);
        runningSound = GetComponents<AudioSource>()[0];
        boostSound = GetComponents<AudioSource>()[1];
        wallrunSound = GetComponents<AudioSource>()[2];
        grindingSound = GetComponents<AudioSource>()[3];
    }
    
    void Update()
    {
        // don't crash if any of these are null
        if (!myPlayerMovement) return;
        //if (!myWPD) return;
        //if (!myBoost) return;
        //if (!runningSound) return;
        //if (!boostSound) return;

        //if (myWPD.inAir)
        if (!myPlayerMovement.isGrounded)
        {
            runningSound.volume = 0;
        }
        else
        {
            runningSound.volume = runningSoundVolume;
        }

        if (myPlayerMovement.isWallRunning)
        {
            wallrunSound.volume = wallrunSoundVolume;
        } else
        {
            wallrunSound.volume = 0;
        }

        if (myPlayerMovement.isGrinding)
        {
            grindingSound.volume = grindingSoundVolume;
        } else
        {
            grindingSound.volume = 0;
        }

        if ((BoostAbility.isBoosting) || 
            (myPlayerMovement&&myPlayerMovement.isBoosting))
        {
            if (!wasBoostingLastFrame)
            {
                boostSound.Play();
            }
        }
        wasBoostingLastFrame = (BoostAbility.isBoosting|| 
            (myPlayerMovement&&myPlayerMovement.isBoosting));

    // debug
    currentRunVol = runningSound.volume;
    currentBoostVol = boostSound.volume;
    currentWallrunVol = wallrunSound.volume;
    currentGrindingVol = grindingSound.volume;


    }
}
