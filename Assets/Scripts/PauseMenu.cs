using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu activated on [ESC] or [P]")]
    public GameObject pauseGUI; 
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Debug.Log("Pausing the game!");
        Time.timeScale = 0f;
        pauseGUI.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("Unpausing the game!");
        Time.timeScale = 1f;
        pauseGUI.SetActive(false);
        isPaused = false;
    }
}




