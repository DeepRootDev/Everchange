using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightManger : MonoBehaviour
{
    public static event System.Action<float> OnFlightValueChange;


    private bool isFlying;
    [SerializeField] private float flightDuration = 5;
    private float flightTimer;

    private void Awake()
    {
        flightTimer = flightDuration;
    }
    public void OnFly(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
        {
            isFlying = true;
        }
        if (ctx.action.WasReleasedThisFrame())
        {
            isFlying = false;
        }
    }

    private void Update()
    {
        if (isFlying  && flightTimer > 0)
        {
            flightTimer -= Time.deltaTime / flightDuration;
            flightTimer = Mathf.Clamp(flightTimer, 0, 1);
            if (flightTimer < 0.1)
                isFlying = false;

            OnFlightValueChange?.Invoke(flightTimer);

        }
        else
        {
            flightTimer = Mathf.Clamp(flightTimer, 0, 1);
            OnFlightValueChange?.Invoke(flightTimer);

            flightTimer += Time.deltaTime / flightDuration;

        }
    }
}
