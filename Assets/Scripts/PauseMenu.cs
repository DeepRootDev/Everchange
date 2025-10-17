using UnityEngine;
using UnityEditor; 
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu activated on [ESC] or [P]")]
    public GameObject pauseGUI;


    [Header("Be sure to include this scene in the project build settings")]
    public string nameOfMainMenuScene = "MainMenu";

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

    public void clickMainMenuButton()
    {
        Debug.Log("Main Menu Button was clicked!");
        SceneManager.LoadScene(nameOfMainMenuScene);
    }
    public void clickRestartButton()
    {
        Debug.Log("Restart Button was clicked!");
        // FIXME: not sure how to reset everything using waypoint drive etc
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        SceneManager.LoadScene(sceneName);
    }

    public void clickExitButton()
    {
        Debug.Log("Exit Button was clicked!");
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}

