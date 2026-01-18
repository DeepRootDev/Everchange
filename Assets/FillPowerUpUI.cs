using UnityEngine;
using UnityEngine.UI;

public class FillPowerUpUI : MonoBehaviour
{
    [SerializeField] private Sprite emptyIcon;
    [SerializeField] private Sprite halfFullIcon;
    [SerializeField] private Sprite fullIcon;

    [SerializeField] private PowerUpItemScriptableObject powerUpItemScriptableObject;

    Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (powerUpItemScriptableObject.NumberOfUsesLeft)
        {
            case 0:
                image.sprite = emptyIcon;
                break;
            case 1:
                image.sprite = halfFullIcon;
                break;
            case 2:
                image.sprite = fullIcon;
                break;
            default:
                image.sprite = emptyIcon;
                break;
        }
    }
}
