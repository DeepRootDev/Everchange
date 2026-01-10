using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using static WaypointDrive;

public class BoostAbility : MonoBehaviour
{

    [SerializeField] private bool allowBoost = false;
    public static event System.Action<float> OnBoostValueChange;
    [SerializeField] private float boostSpeed = 150;
    public static float toReachBoostSpeed = 5;


    public static bool isBoosting = false;
    [SerializeField] private float boostValue = 1;
    [SerializeField] private float boostFillDuration;
    [SerializeField] private float boostDepelateDuration;

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            isBoosting = true;
        }
        if (ctx.action.WasReleasedThisFrame())
        {
            isBoosting = false;
        }
    }

    private void Update()
    {
        if (isBoosting && allowBoost && boostValue > 0)
        {
            boostValue -= Time.deltaTime / boostDepelateDuration;
            boostValue = Mathf.Clamp(boostValue, 0, 1);
            if (boostValue < 0.1)
                isBoosting = false;

            OnBoostValueChange?.Invoke(boostValue);

        }
        else
        {
            boostValue = Mathf.Clamp(boostValue, 0, 1);
            OnBoostValueChange?.Invoke(boostValue);

            boostValue += Time.deltaTime / boostFillDuration;

        }
    }
}
