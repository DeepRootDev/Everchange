using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public PlayerMovement myPlayerMovement;
    public float soundFadeInSpeed = 2f; // percent fade per second so 4 means 0.25 sec from 0 to max
    public float runningSoundVolume = 1f;
    public float boostSoundVolume = 1f;
    public float wallrunSoundVolume = 1f;
    public float grindingSoundVolume = 1f;
    public float glidingSoundVolume = 1f;
    public float jumpSoundVolume = 1f;

    // the waypoint way - FIXME:
    // private WaypointDrive myWPD;
    // private BoostAbility myBoost;
    private AudioSource runningSound;
    private AudioSource boostSound;
    private AudioSource wallrunSound;
    private AudioSource grindingSound;
    private AudioSource glidingSound;
    private AudioSource jumpSound;
    private bool wasBoostingLastFrame = false;

    // only used for debug vis in editor
    public float currentRunVol = 0f;
    public float currentBoostVol = 0f;
    public float currentWallrunVol = 0f;
    public float currentGrindingVol = 0f;

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
        grindingSound = GetComponents<AudioSource>()[3];
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
            if (!grindingSound) grindingSound = GetComponents<AudioSource>()[3];
            if (!glidingSound) glidingSound = GetComponents<AudioSource>()[4];
            if (!jumpSound) jumpSound = GetComponents<AudioSource>()[5];
        } else
        {
            Debug.Log("ERROR: Not enough audiosources on PlayerSounds! We need six: run boost wallrun grind glide jump"); 
            return; // do nothing! no sounds!
        }
        //if (myWPD.inAir)
        if (!myPlayerMovement.isGrounded)
        {
            if (runningSound) runningSound.volume = 0f; // instant
                // fade out
                //Mathf.Lerp(runningSound.volume,0f,soundFadeInSpeed*Time.deltaTime);
        }
        else
        {
            if (runningSound) runningSound.volume = runningSoundVolume;
                // fade in
                //Mathf.Lerp(runningSound.volume,runningSoundVolume,soundFadeInSpeed*Time.deltaTime);
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

        if (myPlayerMovement.isGrinding)
        {
            if (grindingSound) grindingSound.volume = //grindingSoundVolume;
                // fade in
                Mathf.Lerp(grindingSound.volume,grindingSoundVolume,soundFadeInSpeed*Time.deltaTime);
        } else
        {
            if (grindingSound) grindingSound.volume = //0f;
                // fade out
                Mathf.Lerp(grindingSound.volume,0f,soundFadeInSpeed*Time.deltaTime);
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
                if (boostSound) boostSound.Play();
            }
        }
        wasBoostingLastFrame = (BoostAbility.isBoosting|| 
            (myPlayerMovement&&myPlayerMovement.isBoosting));

    // for debug only!!!! 
    currentRunVol = runningSound.volume;
    currentBoostVol = boostSound.volume;
    currentWallrunVol = wallrunSound.volume;
    currentGrindingVol = grindingSound.volume;


    }
}
