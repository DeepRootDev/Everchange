using Unity.VisualScripting;
using UnityEngine;

public class PlayFootStepSound : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] PlayerMovement playerMovement;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (playerMovement.isGrounded) audioSource.Play();
    }
}
