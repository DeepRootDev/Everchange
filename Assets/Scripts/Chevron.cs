using Unity.VisualScripting;
using UnityEngine;

public class Chevron : MonoBehaviour
{
    public Transform tip;
    LineRenderer line;
    float timer;
    private float duration = 1.7f;

    void Awake()
    {
        line = GetComponentInParent<LineRenderer>();
    }

    void Start()
    {
        if (!line || !tip)
            return;
        timer = 0f;
    }

    void LateUpdate()
    {
        if (!line || !tip) 
            return;

            timer += Time.deltaTime;
            float t = (timer / duration) % 1f;
            Vector3 end   = line.GetPosition(1);
            Vector3 start = line.GetPosition(0);
            Vector3 dir = end - start;
            if (dir.sqrMagnitude > 1e-8f)
            {
                Vector3 arrowDir = tip.position - transform.position;
                if (arrowDir.sqrMagnitude < 1e-8f)
                    arrowDir = transform.forward;
                
                Quaternion rot = Quaternion.FromToRotation(arrowDir.normalized, dir.normalized);
                transform.rotation = rot * transform.rotation;
            }               
            transform.position = Vector3.Lerp(start, end, t);

    }
}
