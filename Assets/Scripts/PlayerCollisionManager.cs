using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            //FIXME: Add code for what should occur when the Player collides with an obstacle
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
