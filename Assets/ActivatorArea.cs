using UnityEngine;


public class ActivatorArea : MonoBehaviour
{
    [SerializeField] private Obstacle[] obstacle;
    [SerializeField] private PickUpItemColors areaColor;



    public void Toggle()
    {
        foreach (Obstacle obs in obstacle)
        {
            obs.Toggle();
        }
    }


    public PickUpItemColors GetAreaColor()
    {
        return areaColor;
    }
}