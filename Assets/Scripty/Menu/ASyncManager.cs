using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // <-- NEW for List<>

public class ASyncManager : MonoBehaviour
{
    [Header("Menu Screens")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;

    // ===================== NEW FIELD =====================
    [Header("Random Loading Screens")]
    [Tooltip("If not empty, one of these will be chosen at random instead of using the single 'loadingScreen' reference.")]
    [SerializeField] private List<GameObject> loadingScreens;
    // =====================================================

    [Header("Loading UI Elements")]
    [SerializeField] private RectTransform rotatingIcon;   // The icon you want to rotate
    [SerializeField] private float rotationSpeed = 100f;   // Speed at which the icon rotates
    [SerializeField] private TextMeshProUGUI pressAnyButtonText; // TextMeshProUGUI for �Press any button to continue� text

    public void LoadLevelBtn(string levelToLoad)
    {
        // Hide main menu, show loading screen
        mainMenu.SetActive(false);

        // =============== NEW RANDOM SCREEN LOGIC ===============
        // If we have multiple loading screens, pick one at random
        if (loadingScreens != null && loadingScreens.Count > 0)
        {
            int randomIndex = Random.Range(0, loadingScreens.Count);
            loadingScreen = loadingScreens[randomIndex];
        }
        // ========================================================

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

            // Optionally update a progress bar here, if desired

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
