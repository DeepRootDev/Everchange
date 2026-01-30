using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ActivatorArea : MonoBehaviour
{
    [SerializeField] private Obstacle[] obstacle;
    [SerializeField] private KeyCode keyCode = KeyCode.None;
    [SerializeField] private PickUpItemColors areaColor;

    private InputAction activateAreaAction;
    public static event Action onActivatorActionPerformed;

    void Awake()
    {
        activateAreaAction = new InputAction(name: "ActivatorAreaAction", type: InputActionType.Button);
        activateAreaAction.AddBinding("<Keyboard>/" + keyCode.ToString());
        activateAreaAction.performed += OnActivateArea;
        activateAreaAction.Enable();
    }
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

    private void OnActivateArea(InputAction.CallbackContext ctx)
    {
        if (activateAreaAction.WasPerformedThisFrame())
        {
            onActivatorActionPerformed?.Invoke();   
        }
    }

    void OnDestroy()
    {
        activateAreaAction.Disable();
        activateAreaAction.Dispose();
    }

}
