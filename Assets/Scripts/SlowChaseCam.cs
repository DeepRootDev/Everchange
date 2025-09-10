using UnityEngine;

public class SlowChase : MonoBehaviour
{
    public Transform chaseTarget;
    public Transform lookPoint;

    private float posHalfLife = 0.05f;
    private float rotHalfLife = 0.10f;

    private void LateUpdate()
    {
        // messy because I need it smooth for camera movement (can't do FixedUpdate)
        // but since lerp and slerp aren't linear just multiplying by time.deltaTime would break things
        // more thorough explanation of why this is messy math: https://medium.com/@tglaiel/how-to-make-your-game-run-at-60fps-24c61210fe75
        float posT = Mathf.Clamp01(1f - Mathf.Pow(0.5f, Time.deltaTime / Mathf.Max(Mathf.Epsilon, posHalfLife)));
        float rotT = Mathf.Clamp01(1f - Mathf.Pow(0.5f, Time.deltaTime / Mathf.Max(Mathf.Epsilon, rotHalfLife)));

        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(lookPoint.position - transform.position), rotT);

        transform.position = Vector3.Slerp(transform.position, chaseTarget.position, posT);

    }
}
