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
            playerVoicePlayerBox.PlaySound(soundToPlay);
        }
    }
}
