using UnityEngine;

public class PlayerSoundTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioSource soundToPlay;
    public string b;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            soundToPlay.Play();
        }
    }
}
