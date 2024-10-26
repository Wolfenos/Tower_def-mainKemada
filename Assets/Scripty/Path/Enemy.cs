using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class Enemy : MonoBehaviour
    {
        public float maxHealth = 100f;
        private float health;
        public GameObject healthBarPrefab;
        private GameObject healthBarInstance;
        private Slider healthBarSlider;
        public Vector3 healthBarOffset = new Vector3(0, 2, 0);

        public float moveSpeed = 5f;
        private float currentSpeedModifier = 1f;
        private Transform path;
        private Transform[] pathPoints;
        private int currentPointIndex = 0;
        private float groundY;  // Fixed Y position based on ground level

        public void Initialize(float initialHealth, Transform enemyPath)
        {
            maxHealth = initialHealth;
            health = maxHealth;

            // Set the path
            if (enemyPath != null)
            {
                path = enemyPath;
                pathPoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                {
                    pathPoints[i] = path.GetChild(i);
                }
            }

            // Set ground Y position based on initial spawn point
            groundY = transform.position.y;

            // Instantiate and set up the health bar
            healthBarInstance = Instantiate(healthBarPrefab);
            healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
            UpdateHealthBar();
        }

        private void Update()
        {
            if (pathPoints != null && pathPoints.Length > 0)
            {
                MoveAlongPath();
            }

            if (healthBarInstance != null)
            {
                healthBarInstance.transform.position = transform.position + healthBarOffset;
                healthBarInstance.transform.LookAt(Camera.main.transform);
            }
        }

        // Method to move the enemy along the path while staying on ground level
        private void MoveAlongPath()
        {
            if (currentPointIndex >= pathPoints.Length)
            {
                return;
            }

            Transform targetPoint = pathPoints[currentPointIndex];
            Vector3 direction = (targetPoint.position - transform.position).normalized;

            // Only modify the X and Z positions, keeping Y fixed to ground level
            Vector3 newPosition = transform.position + direction * moveSpeed * currentSpeedModifier * Time.deltaTime;
            newPosition.y = groundY;

            transform.position = newPosition;

            // Check if the enemy has reached the current point
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++;
            }
        }

        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + health);

            UpdateHealthBar();

            if (health <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log(gameObject.name + " has been destroyed!");

            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }

            Destroy(gameObject);
        }

        private void UpdateHealthBar()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = health / maxHealth;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("SlowGround") || other.CompareTag("NormalGround"))
            {
                GroundController ground = other.GetComponent<GroundController>();
                if (ground != null)
                {
                    currentSpeedModifier = ground.speedModifier;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("SlowGround") || other.CompareTag("NormalGround"))
            {
                currentSpeedModifier = 1f;
            }
        }
    }
}
