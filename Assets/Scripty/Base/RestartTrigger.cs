using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestartTrigger : MonoBehaviour
{
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private string menuSceneName = "Menu";

    [SerializeField] private int baseHealth = 10;    // Health can be set in Inspector
    [SerializeField] private TMP_Text healthText;    // Link to TextMesh Pro component

    private void Start()
    {
        // Make the trigger collider non-clickable and non-interactable by the player
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Initialize the health display if healthText is assigned
        if (healthText != null)
        {
            healthText.text = baseHealth.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object with tag '{other.tag}' entered the trigger.");

        if (other.CompareTag(targetTag))
        {
            Debug.Log("Enemy entered the trigger.");

            // 1) Subtract one health
            baseHealth--;

            // 2) Update UI if assigned
            if (healthText != null)
            {
                healthText.text = baseHealth.ToString();
            }

            // 3) Destroy the enemy object
            Destroy(other.gameObject);

            // 4) If health is depleted, switch to the specified scene
            if (baseHealth <= 0)
            {
                Debug.Log("Base health is depleted. Switching scene...");
                SwitchScene();
            }
        }
        else
        {
            Debug.LogWarning($"Object with tag '{other.tag}' does not match the target tag '{targetTag}'.");
        }

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("The object that entered the trigger does not have a Rigidbody. " +
                             "A Rigidbody is required for proper trigger interaction.");
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
