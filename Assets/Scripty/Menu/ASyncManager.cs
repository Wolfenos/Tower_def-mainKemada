using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ASyncManager : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    [Header("Loading UI Elements")]
    [SerializeField] private RectTransform rotatingIcon;   // The icon you want to rotate
    [SerializeField] private float rotationSpeed = 100f;   // Speed at which the icon rotates
    [SerializeField] private TextMeshProUGUI pressAnyButtonText; // TextMeshProUGUI for “Press any button to continue” text

    public void LoadLevelBtn(string levelToLoad)
    {
        // Hide main menu, show loading screen
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        // Make sure the "press any button" text is hidden at first
        if (pressAnyButtonText != null)
        {
            pressAnyButtonText.gameObject.SetActive(false);
        }

        // Start the async loading coroutine
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        // Start loading the scene but don't allow it to activate immediately
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelToLoad);
        asyncOperation.allowSceneActivation = false;

        // While the scene is still loading but hasn't hit 0.9f (which is effectively "loaded")
        while (asyncOperation.progress < 0.9f)
        {
            // Rotate the icon, if assigned
            if (rotatingIcon != null)
            {
                rotatingIcon.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }

            // Optionally: If you have a progress bar or text, you can update it here:
            // float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            // progressBar.fillAmount = progressValue;

            yield return null;
        }

        // Loading is basically done, so show "Press any button to continue"
        if (pressAnyButtonText != null)
        {
            pressAnyButtonText.gameObject.SetActive(true);
        }

        // Keep rotating the icon while we wait for user input
        while (!Input.anyKeyDown)
        {
            if (rotatingIcon != null)
            {
                rotatingIcon.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
            yield return null;
        }

        // Once the user presses a key, allow the scene to activate
        asyncOperation.allowSceneActivation = true;
    }
}
