using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using KemadaTD; // <-- Important if WaveUIEnhancer/Enemy are in namespace KemadaTD

public class RestartTrigger : MonoBehaviour
{
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private string menuSceneName = "Menu";

    [SerializeField] private int baseHealth = 10;
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        // Make the trigger collider non-interactable
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        if (healthText != null)
        {
            healthText.text = baseHealth.ToString();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Reduce base health
            baseHealth--;
            if (healthText != null) healthText.text = baseHealth.ToString();

            // Get the Enemy component (if any)
            Enemy enemyComponent = other.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                // Manually broadcast "OnEnemyDied" so that WaveUIEnhancer living-enemy count is decremented
                Enemy.OnEnemyDied?.Invoke(enemyComponent);
            }

            // Destroy the enemy
            Destroy(other.gameObject);

            // Check if base is dead
            if (baseHealth <= 0)
            {
                SwitchScene();
            }
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
            Debug.LogError($"Scene '{menuSceneName}' cannot be loaded. " +
                           "Make sure it is added to the build settings.");
        }
    }
}
