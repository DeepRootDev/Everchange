using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public PlayerMovement myPlayerMovement;
    public float soundFadeInSpeed = 2f; // percent fade per second so 4 means 0.25 sec from 0 to max
    public float runningSoundVolume = 1f;
    public float boostSoundVolume = 1f;
    public float wallrunSoundVolume = 1f;
    public float driftingSoundVolume = 1f;
    public float glidingSoundVolume = 1f;
    public float jumpSoundVolume = 1f;

    // the waypoint way - FIXME:
    // private WaypointDrive myWPD;
    // private BoostAbility myBoost;
    private AudioSource runningSound;
    private AudioSource boostSound;
    private AudioSource wallrunSound;
    private AudioSource driftingSound;
    private AudioSource glidingSound;
    private AudioSource jumpSound;
    private bool wasBoostingLastFrame = false;

    // only used for debug vis in editor
    public float currentRunVol = 0f;
    public float currentBoostVol = 0f;
    public float currentWallrunVol = 0f;
    public float currentDriftingVol = 0f;

    // not Awake - this is fired twice?!
    void Start()
    {
        // FIXME: we get strange errors here on the first frame
        // as if unity has not finished loading these sounds
        // but after inits it works!!
        // seems like Start() is being called TWICE?
        Debug.Log("We need 6 audiosources (run boost, wallrun, grind, glide, jump) and have "+GetComponents<AudioSource>().Length);
        runningSound = GetComponents<AudioSource>()[0];
        boostSound = GetComponents<AudioSource>()[1];
        wallrunSound = GetComponents<AudioSource>()[2];
        driftingSound = GetComponents<AudioSource>()[3];
        glidingSound = GetComponents<AudioSource>()[4];
        jumpSound = GetComponents<AudioSource>()[5];
    }
    
    void Update()
    {
        // don't crash if any of these are null
        if (!myPlayerMovement) return;

        // during the awake and start events, we don't always have all six
        // so keep trying until they are ready to use
        if (GetComponents<AudioSource>().Length==6) {
            if (!runningSound) runningSound = GetComponents<AudioSource>()[0];
            if (!boostSound) boostSound = GetComponents<AudioSource>()[1];
            if (!wallrunSound) wallrunSound = GetComponents<AudioSource>()[2];
            if (!driftingSound) driftingSound = GetComponents<AudioSource>()[3];
            if (!glidingSound) glidingSound = GetComponents<AudioSource>()[4];
            if (!jumpSound) jumpSound = GetComponents<AudioSource>()[5];
        } else
        {
            Debug.Log("ERROR: Not enough audiosources on PlayerSounds! We need six: run boost wallrun grind glide jump"); 
            return; // do nothing! no sounds!
        }

        if (runningSound!=null) {
            if (!myPlayerMovement.isGrounded) {
                runningSound.volume = 0f; // no footstep sfx when in the air
            } else  {
                if (myPlayerMovement.currentSpeed > 0.25) {
                    runningSound.volume = runningSoundVolume;
                } else {
                    runningSound.volume = 0; // not moving
                }
            }
        }

        if (myPlayerMovement.isWallRunning)
        {
            if (wallrunSound) wallrunSound.volume = //wallrunSoundVolume;
                // fade in
                Mathf.Lerp(wallrunSound.volume,wallrunSoundVolume,soundFadeInSpeed*Time.deltaTime);
        } else
        {
            if (wallrunSound) wallrunSound.volume = //0f;
                // fade out
                Mathf.Lerp(wallrunSound.volume,0f,soundFadeInSpeed*Time.deltaTime);
        }

        if (myPlayerMovement.isDrifting)
        {
            if (driftingSound) driftingSound.volume = //grindingSoundVolume;
                // fade in
                Mathf.Lerp(driftingSound.volume,driftingSoundVolume,soundFadeInSpeed*Time.deltaTime);
        } else
        {
            if (driftingSound) driftingSound.volume = //0f;
                // fade out
                Mathf.Lerp(driftingSound.volume,0f,soundFadeInSpeed*Time.deltaTime);
        }

        if (myPlayerMovement.isGliding)
        {
            if (glidingSound) glidingSound.volume = //glidingSoundVolume;
                // fade in
                Mathf.Lerp(glidingSound.volume,glidingSoundVolume,soundFadeInSpeed*Time.deltaTime);
        } else
        {
            if (glidingSound) glidingSound.volume = //0f;
                // fade out
                Mathf.Lerp(glidingSound.volume,0f,soundFadeInSpeed*Time.deltaTime);
        }

        if (myPlayerMovement.justJumped)
        {
            if (jumpSound) jumpSound.volume = jumpSoundVolume;
            if (jumpSound) jumpSound.Play();
        }

        if ((BoostAbility.isBoosting) || 
            (myPlayerMovement&&myPlayerMovement.isBoosting))
        {
            if (!wasBoostingLastFrame)
            {
                if (boostSound) {
                    Debug.Log("Playing boost sound!");
                    boostSound.volume = boostSoundVolume;
                    boostSound.Play();
                }
                wasBoostingLastFrame = true;
            }
        } else {
            wasBoostingLastFrame = false;
        }

    // for debug only!!!! 
    currentRunVol = runningSound.volume;
    currentBoostVol = boostSound.volume;
    currentWallrunVol = wallrunSound.volume;
    currentDriftingVol = driftingSound.volume;


    }
}
