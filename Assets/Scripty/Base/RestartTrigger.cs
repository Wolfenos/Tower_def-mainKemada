using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartTrigger : MonoBehaviour
{
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private float timeBeforeSwitch = 10.0f;

    private void Start()
    {
        // Start coroutine to switch to the menu scene after a set time
        StartCoroutine(SwitchSceneAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("Enemy entered the trigger.");
        }
    }

    private IEnumerator SwitchSceneAfterTime()
    {
        yield return new WaitForSeconds(timeBeforeSwitch);
        if (Application.CanStreamedLevelBeLoaded(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{menuSceneName}' cannot be loaded. Make sure it is added to the build settings.");
        }
    }
}