using UnityEngine;

public class PlayerSoundTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip soundToPlay;
    [SerializeField]
    private PlayerVoicePlayerBox playerVoicePlayerBox;

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
                playerVoicePlayerBox.PlaySound(soundToPlay);
            }
            
        }
    }
}
