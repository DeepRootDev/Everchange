using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EverchangeSceneManager : MonoBehaviour
{
    [TextArea] [SerializeField] private string note = "Contains logic to switch between scenes. Enter the names of the scenes down below. Names must with the scenes in the scenes/ folder!";
    
    [SerializeField] private string loadingSceneName = "Loading Scene";
    [SerializeField] private float timeBeforeLoadingScene = 0.1f; // small time to wait to allow for hearing button sounds 
    
    public void LoadLoadingScene()
    {
        StartCoroutine(LoadSceneAfterSecond(loadingSceneName));
    }

    // debug version only
    public void LoadTestPlayground()
    {
        SceneManager.LoadScene("Test Playground");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneAfterSecond(string sceneName)
    {
        yield return new WaitForSeconds(timeBeforeLoadingScene);
        SceneManager.LoadScene(loadingSceneName);
    }

}