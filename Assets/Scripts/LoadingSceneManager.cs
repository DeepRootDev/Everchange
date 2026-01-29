using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// NOTE(marvin): It is separate from the EverchangeSceneManager because of the extra logic with loading the next
// scene asynchronously and updating the percentage.
public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private GameObject loadingPanelGameObject;
    [SerializeField] private GameObject continuePanelGameObject;

    [SerializeField] private string gameScene;

    private AsyncOperation loadOperation;

    public InputActionAsset inputActions;
    private InputAction uiClickAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        PresentContinueButton(false);
        StartCoroutine(LoadGameSceneAsync());

        inputActions.FindActionMap("UI").Enable();
        uiClickAction = InputSystem.actions.FindAction("UI/Click");
    }

    private void Update()
    {
        if (loadOperation != null && ReadyForNextScene() &&
            uiClickAction.WasPressedThisFrame())
        {
            ContinueToNextScene();
        }
    }

    // Should only be called when load operation is not null.
    private bool ReadyForNextScene()
    {
        // NOTE(marvin): We are relying on allowSceneActivation being false which makes the load operation freeze at
        // 90%. Once we hit that point, we know that the game is ready for the next scene.
        // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AsyncOperation-allowSceneActivation.html
        bool result = loadOperation.progress >= 0.9f;
        return result;
    }

    // Updates the percentage text every frame.
    private IEnumerator LoadGameSceneAsync()
    {
        loadOperation = SceneManager.LoadSceneAsync(gameScene);
        yield return null;
        
        loadOperation.allowSceneActivation = false;

        while (!ReadyForNextScene())
        {
            // NOTE(marvin): The division by 0.9f is explained by the note in ReadyForNextScene().
            int percentageProgressInt =  (int)((loadOperation.progress / 0.9f) * 100.0f);
            textComponent.text = $"{percentageProgressInt}%";
            yield return null;
        }
        
        PresentContinueButton(true);
    }

    private void PresentContinueButton(bool should)
    {
        loadingPanelGameObject.SetActive(!should);
        continuePanelGameObject.SetActive(should);
    }

    private void ContinueToNextScene()
    {
        loadOperation.allowSceneActivation = true;
    }
}
