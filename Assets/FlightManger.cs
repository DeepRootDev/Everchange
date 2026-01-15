using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightManger : MonoBehaviour
{
    public static event System.Action<float> OnFlightValueChange;


    private bool isFlying = false;
    [SerializeField] private float flightDuration = 5;
    private float flightTimer;

    private void Awake()
    {
        flightTimer = flightDuration;
    }
    void OnFly(InputValue value)
    {
        isFlying = value.isPressed;
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
