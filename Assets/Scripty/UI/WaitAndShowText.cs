using UnityEngine;
using UnityEngine.UI; // Remove or change if using TextMeshPro

public class WaitAndShowText : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many seconds to wait before showing text and enabling the button.")]
    public float waitTime = 5f;

    [Header("References")]
    [Tooltip("The UI Text (or TextMeshPro) object that should appear after the wait.")]
    public GameObject textObject;

    [Tooltip("The button that should be enabled after the wait.")]
    public Button buttonToEnable;

    private void Start()
    {
        // Make sure text is hidden and button is disabled at start
        if (textObject != null)
            textObject.SetActive(false);

        if (buttonToEnable != null)
            buttonToEnable.interactable = false;

        // Start the coroutine to wait and then show text/enable button
        StartCoroutine(WaitAndActivate());
    }

    private System.Collections.IEnumerator WaitAndActivate()
    {
        // Wait the specified amount of time
        yield return new WaitForSeconds(waitTime);

        // After waiting, show the text
        if (textObject != null)
            textObject.SetActive(true);

        // Enable the button
        if (buttonToEnable != null)
            buttonToEnable.interactable = true;
    }
}
