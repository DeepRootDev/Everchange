using UnityEngine;

public class PlayerSoundTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip soundToPlay;
    [SerializeField]
    private float soundVolume;
    [SerializeField]
    private PlayerVoicePlayerBox playerVoicePlayerBox;

    private bool soundPlayedYet = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playerVoicePlayerBox)
            {
                Debug.Log("There is no player voice player box attached to player sound trigger");
            }
            else 
            {
                if(soundPlayedYet)
                {
                    Debug.Log("prevented audio from double playing");
                    return;
                }
                soundPlayedYet = true;
                playerVoicePlayerBox.PlaySound(soundToPlay, soundVolume);
            }
            
        }
    }
}
