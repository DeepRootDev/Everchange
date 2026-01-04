using UnityEngine;

public class ActivatorArea : MonoBehaviour
{
    [SerializeField] private Obstacle[] obstacle;
    [SerializeField] private KeyCode keyCode = KeyCode.None;
    [SerializeField] private PickUpItemColors areaColor;

    public void Toggle()
    {
        foreach (Obstacle obs in obstacle)
        {
            obs.Toggle();
        }
    }

    public KeyCode GetKeyType()
    {
        return keyCode;
    }

    public PickUpItemColors GetAreaColor()
    {
        return areaColor;
    }

}
