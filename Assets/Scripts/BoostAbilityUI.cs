using UnityEngine;
using UnityEngine.UI;

public class BoostAbilityUI : MonoBehaviour
{
    [SerializeField] private Image boostFillImage;


    private void Start()
    {
        BoostAbility.OnBoostValueChange += WaypointDrive_OnBoostValueChange;
    }

    private void OnDestroy()
    {
        BoostAbility.OnBoostValueChange -= WaypointDrive_OnBoostValueChange;
    }

    private void WaypointDrive_OnBoostValueChange(float boostValue)
    {
        boostFillImage.fillAmount = boostValue;

    }
}
