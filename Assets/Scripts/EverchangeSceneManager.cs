using UnityEngine;
using UnityEngine.SceneManagement;

public class EverchangeSceneManager : MonoBehaviour
{
    [TextArea] [SerializeField] private string note = "Contains logic to switch between scenes. Enter the names of the scenes down below. Names must with the scenes in the scenes/ folder!";
    
    [SerializeField] private string loadingSceneName = "Loading Scene";
    
    public void LoadLoadingScene()
    {
        SceneManager.LoadScene(loadingSceneName);
    }
}