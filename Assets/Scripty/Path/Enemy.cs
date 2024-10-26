using UnityEngine;

namespace KemadaTD
{
    public class Enemy : MonoBehaviour
    {
        public GameObject healthBarPrefab;  // Reference to health bar prefab (no health management inside)
        public float moveSpeed = 5f;

        private Transform path;
        private Transform[] pathPoints;
        private int currentPointIndex = 0;
        private float groundY;
        private float currentSpeedModifier = 1f;

        private HealthBar healthBar;  // Reference to HealthBar component

        // Initialize enemy with path and health bar setup
        public void Initialize(float initialHealth, Transform enemyPath)
        {
            if (enemyPath != null)
            {
                path = enemyPath;
                pathPoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                {
                    pathPoints[i] = path.GetChild(i);
                }
            }

            groundY = transform.position.y;

            // Instantiate the health bar and set it up
            GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBar = healthBarInstance.GetComponent<HealthBar>();

            if (healthBar != null)
            {
                healthBar.Initialize(initialHealth, transform);  // Pass the enemy's transform to HealthBar
            }
            else
            {
                Debug.LogError("HealthBar component missing from HealthBar prefab.");
            }
        }

        private void Update()
        {
            if (pathPoints != null && pathPoints.Length > 0)
            {
                MoveAlongPath();
            }
        }

        private void MoveAlongPath()
        {
            if (currentPointIndex >= pathPoints.Length)
            {
                return;
            }

            Transform targetPoint = pathPoints[currentPointIndex];
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * moveSpeed * currentSpeedModifier * Time.deltaTime;
            newPosition.y = groundY;

            transform.position = newPosition;

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                currentPointIndex++;
            }
        }

        // Public method to deal damage to the enemy
        public void TakeDamage(float damageAmount)
        {
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageAmount);  // Delegate to HealthBar without managing health directly
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
