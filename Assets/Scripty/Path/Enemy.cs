using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class Enemy : MonoBehaviour
    {
        public float maxHealth = 100f;         // Maximum health
        private float health;                  // Current health
        public GameObject healthBarPrefab;     // Prefab for the health bar UI
        private GameObject healthBarInstance;  // Instance of the health bar UI
        private Slider healthBarSlider;        // Reference to the slider component in the health bar
        public Vector3 healthBarOffset = new Vector3(0, 2, 0); // Offset for the health bar's position

        public float moveSpeed = 5f;           // Speed of the enemy movement
        private Transform path;                // Reference to the path (enemy movement)
        private Transform[] pathPoints;        // List of points in the path
        private int currentPointIndex = 0;     // Current point the enemy is moving towards

        // This method initializes the enemy with health and a path to follow
        public void Initialize(float initialHealth, Transform enemyPath)
        {
            maxHealth = initialHealth;
            health = maxHealth;

            // Set the path
            if (enemyPath != null)
            {
                path = enemyPath;
                pathPoints = new Transform[path.childCount]; // Assuming path points are children of the path
                for (int i = 0; i < path.childCount; i++)
                {
                    pathPoints[i] = path.GetChild(i);
                }
            }

            // Instantiate and set up the health bar
            healthBarInstance = Instantiate(healthBarPrefab);
            healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
            UpdateHealthBar();
        }

        private void Update()
        {
            // Handle movement along the path
            if (pathPoints != null && pathPoints.Length > 0)
            {
                MoveAlongPath();
            }

            // Update health bar position
            if (healthBarInstance != null)
            {
                healthBarInstance.transform.position = transform.position + healthBarOffset;
                healthBarInstance.transform.LookAt(Camera.main.transform);
            }
        }

        // Method to move the enemy along the path
        private void MoveAlongPath()
        {
            if (currentPointIndex >= pathPoints.Length)
            {
                // Enemy has reached the end of the path (you can add logic here for what happens next)
                return;
            }

            // Move towards the next path point
            Transform targetPoint = pathPoints[currentPointIndex];
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

            // Check if the enemy has reached the current point
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++; // Move to the next point in the path
            }
        }

        // Method to take damage, called by Bullet when hit
        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + health);

            UpdateHealthBar(); // Update the health bar UI

            if (health <= 0f)
            {
                Die();
            }
        }

        // Method to destroy the enemy and the health bar
        private void Die()
        {
            Debug.Log(gameObject.name + " has been destroyed!");

            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }

            Destroy(gameObject); // Destroy the enemy gameObject
        }

        // Method to update the health bar UI
        private void UpdateHealthBar()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = health / maxHealth;
            }
        }
    }
}