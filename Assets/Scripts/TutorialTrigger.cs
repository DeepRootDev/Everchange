using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] GameObject rootTutorialObject;
    [SerializeField] GameObject tutorialToDisplay;
    [SerializeField] float displayDuration = 3f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() == null) return;

        StartCoroutine(DisplayTutorialForDuration());
    }

    private IEnumerator DisplayTutorialForDuration()
    {
        rootTutorialObject.SetActive(true);
        foreach(Transform child in rootTutorialObject.transform)
        {
            child.gameObject.SetActive(false);
        }
        tutorialToDisplay.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        tutorialToDisplay.SetActive(false);
        rootTutorialObject.SetActive(false);
    }
}
