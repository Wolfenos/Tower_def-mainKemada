using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrigger : MonoBehaviour
{
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        // Make the trigger collider non-clickable and non-interactable by player
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object with tag '{other.tag}' entered the trigger.");

        if (other.CompareTag(targetTag))
        {
            Debug.Log("Enemy entered the trigger.");
            SwitchScene();
        }
        else
        {
            Debug.LogWarning($"Object with tag '{other.tag}' does not match the target tag '{targetTag}'.");
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("The object that entered the trigger does not have a Rigidbody. A Rigidbody is required for proper trigger interaction.");
        }
    }

    private void SwitchScene()
    {
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