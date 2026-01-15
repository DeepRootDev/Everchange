using UnityEngine;
using UnityEngine.InputSystem;

public class MuteHandler : MonoBehaviour
{
    private bool isMuted = false;
    //private AudioListener audioListener;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //audioListener = FindFirstObjectByType<AudioListener>();
    }

    public void OnToggleMute(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isMuted = !isMuted;
            AudioListener.pause = isMuted;
            Debug.Log(isMuted? "Game Muted": "Game Unmuted");
        }        
    }
}
