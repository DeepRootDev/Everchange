using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField ] private bool isActive = true;

    protected bool IsActive { get => isActive; set => isActive = value; }
    
    public virtual void Toggle()
    {
        IsActive = !IsActive;
    }
}
