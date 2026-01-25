using UnityEngine;
using UnityEditor; 
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu activated on [ESC] or [P]")]
    public GameObject pauseGUI;


    [Header("Be sure to include this scene in the project build settings")]
    public string nameOfMainMenuScene = "MainMenu";

    public static PauseMenu instance;

    public bool isPaused = false;

    [Header("Input Action Asset")]
    public InputActionAsset inputActions;
    private InputAction pauseActionUI;
    private InputAction playerPauseAction;
    private InputActionMap playerActionMap;


    void Awake()
    {
        instance = this;
        playerPauseAction = InputSystem.actions.FindAction("Player/Pause");
        pauseActionUI = InputSystem.actions.FindAction("UI/Resume");
        playerActionMap = inputActions.FindActionMap("Player");
        
        pauseActionUI.Disable();
    }

    void Update()
    {
        if (playerPauseAction.WasPressedThisFrame())
        {
            PauseGame();
        }
        else if (pauseActionUI.WasPressedThisFrame())
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("Pausing the game!");
        Time.timeScale = 0f;
        pauseGUI.SetActive(true);
        playerActionMap.Disable();
        pauseActionUI.Enable();
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Debug.Log("Unpausing the game!");
        Time.timeScale = 1f;
        pauseGUI.SetActive(false);
        playerActionMap.Enable();
        pauseActionUI.Disable();
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void clickMainMenuButton()
    {
        Debug.Log("Main Menu Button was clicked!");
        SceneManager.LoadScene(nameOfMainMenuScene);
    }
    public void clickRestartButton()
    {
        Debug.Log("Restart Button was clicked!");
        Time.timeScale = 1f;
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

