using UnityEngine;
using UnityEngine.Serialization;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenuGameObject;
    [SerializeField] private Transform menuFontTransform;
    [SerializeField] private GameObject optionsButtonGameObject;

    private bool optionsToggled = false;
    
    public void ToggleOptionsMenu()
    {
        if (optionsToggled)
        {
            // NOTE(marvin): It's currently opened, and we want to close it.
            optionsMenuGameObject.SetActive(false);

            foreach (Transform menuItemTransform in menuFontTransform)
            {
                GameObject menuItemGameObject = menuItemTransform.gameObject;
                menuItemGameObject.SetActive(true);
            }
        }
        else
        {
            // NOTE(marvin): It's currently closed, and we want to open it.
            optionsMenuGameObject.SetActive(true);
            
            foreach (Transform menuItemTransform in menuFontTransform)
            {
                GameObject menuItemGameObject = menuItemTransform.gameObject;
                bool isOptionsButton = menuItemGameObject == optionsButtonGameObject;
                menuItemGameObject.SetActive(isOptionsButton);
            }
        }
        
        optionsToggled = !optionsToggled;
    }
}