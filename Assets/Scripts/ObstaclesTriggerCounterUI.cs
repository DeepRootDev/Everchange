using TMPro;
using UnityEngine;

public class ObstaclesTriggerCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redColorText;
    [SerializeField] private TextMeshProUGUI blueColorText;

    private void Start()
    {
        ObstaclesTriggerCounter.OnTriggerValueChanged += ObstaclesTriggerCounter_OnTriggerValueChanged;
    }

    private void OnDestroy()
    {
        ObstaclesTriggerCounter.OnTriggerValueChanged -= ObstaclesTriggerCounter_OnTriggerValueChanged;
    }

    private void ObstaclesTriggerCounter_OnTriggerValueChanged(int red,int blue)
    {
        redColorText.text = red.ToString();
        blueColorText.text = blue.ToString();
    }
}
