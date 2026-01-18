using UnityEngine;

[CreateAssetMenu(fileName = "newPowerUp", menuName = "PowerUp")]
public class PowerUpItemScriptableObject : ScriptableObject
{
    public PickUpItemColors Color;
    public const int MaxNumberOfUses = 2;
    [SerializeField]
    [Range(0, MaxNumberOfUses)]
    private int _numberOfUsesLeft;

    public int NumberOfUsesLeft
    {
        get
        {
            return _numberOfUsesLeft;
        }
        set
        {
            if (value >= MaxNumberOfUses)
                value = MaxNumberOfUses;
            else if (value <= 0)
                value = 0;
            _numberOfUsesLeft = value; 
        }
    }
}
