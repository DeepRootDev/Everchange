using UnityEngine;
using UnityEngine.UI;

public class FlightIconUI : MonoBehaviour
{
    [SerializeField] private Image flightIcon;

    private void Awake()
    {
        flightIcon = GetComponent<Image>();
    }

    private void Start()
    {
        FlightManger.OnFlightValueChange += FlightManger_OnFlightValueChange;
    }

    private void FlightManger_OnFlightValueChange(float obj)
    {
        flightIcon.color = new Color(flightIcon.color.r,flightIcon.color.g,flightIcon.color.b, obj  );
    }

    private void OnDestroy()
    {
        FlightManger.OnFlightValueChange -= FlightManger_OnFlightValueChange;
    }
}
