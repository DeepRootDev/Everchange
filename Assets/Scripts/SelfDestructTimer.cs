using UnityEngine;

public class SelfDestructTimer : MonoBehaviour
{
    public float timeAlive = 5.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,timeAlive);
    }
}
