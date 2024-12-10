using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class Enemy : MonoBehaviour
    {
        public float maxHealth = 100f;
        private float health;
        public Vector3 healthBarOffset = new Vector3(0, 2, 0);

        public float moveSpeed = 5f;
        private float currentSpeedModifier = 1f;
        private Transform path;
        private Transform[] pathPoints;
        private int currentPointIndex = 0;

        public GameObject damageNumberPrefab;
        public Healthbarv2 healthbar;

        // For LERP movement
        private float lerpProgress = 0f;
        private float totalDistanceToNextPoint = 0f;
        private Vector3 startPosition;

        [Header("Ground Check")]
        public LayerMask groundLayer;
        private float raycastHeight = 100f; // High above the map to ensure we always cast down to find ground

        public void Initialize(float initialHealth, Transform enemyPath)
        {
            maxHealth = initialHealth;
            health = maxHealth;

            if (enemyPath != null)
            {
                path = enemyPath;
                pathPoints = new Transform[path.childCount];
                for (int i = 0; i < path.childCount; i++)
                {
                    pathPoints[i] = path.GetChild(i);
                }

                // We'll move along pathPoints in order: 0, 1, 2, ...
                // Make sure you have at least one point.
                if (pathPoints.Length > 0)
                {
                    // The enemy should already be spawned on a spawn point by the WaveManager.
                    // Set the starting position for the first segment
                    startPosition = transform.position;
                    SetNewTarget(pathPoints[0]);
                }
                else
                {
                    Debug.LogWarning("No path points found. Enemy will not move.");
                }
            }

            if (healthbar != null)
            {
                healthbar.UpdateHealth(health / maxHealth);
            }
        }

        private void Update()
        {
            if (pathPoints != null && currentPointIndex < pathPoints.Length)
            {
                MoveAlongPath();
            }

            if (healthbar != null)
            {
                healthbar.transform.position = transform.position + healthBarOffset;
                healthbar.transform.LookAt(Camera.main.transform);
            }
        }

        private void MoveAlongPath()
        {
            if (currentPointIndex >= pathPoints.Length) return;

            Transform targetPoint = pathPoints[currentPointIndex];
            if (targetPoint == null) return;

            // Increment lerp progress based on horizontal travel distance
            float step = (moveSpeed * currentSpeedModifier * Time.deltaTime) / totalDistanceToNextPoint;
            lerpProgress += step;

            // Compute horizontal (XZ) lerp:
            Vector2 startXZ = new Vector2(startPosition.x, startPosition.z);
            Vector2 endXZ = new Vector2(targetPoint.position.x, targetPoint.position.z);
            Vector2 currentXZ = Vector2.Lerp(startXZ, endXZ, lerpProgress);

            // Now find correct Y via raycast
            Vector3 positionToAdjust = new Vector3(currentXZ.x, raycastHeight, currentXZ.y);

            if (Physics.Raycast(positionToAdjust, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                positionToAdjust.y = hit.point.y;
            }
            else
            {
                // If no ground, keep current Y (unlikely if groundLayer is correct)
                positionToAdjust.y = transform.position.y;
                Debug.LogWarning("No ground detected below the enemy. Check ground colliders and layers.");
            }

            transform.position = positionToAdjust;

            // Check if we've completed this segment
            if (lerpProgress >= 1f)
            {
                currentPointIndex++;
                if (currentPointIndex < pathPoints.Length)
                {
                    startPosition = transform.position;
                    SetNewTarget(pathPoints[currentPointIndex]);
                    lerpProgress = 0f;
                }
            }
        }

        private void SetNewTarget(Transform nextPoint)
        {
            if (nextPoint != null)
            {
                // Calculate horizontal distance only
                Vector2 startXZ = new Vector2(startPosition.x, startPosition.z);
                Vector2 endXZ = new Vector2(nextPoint.position.x, nextPoint.position.z);
                totalDistanceToNextPoint = Vector2.Distance(startXZ, endXZ);

                if (Mathf.Approximately(totalDistanceToNextPoint, 0f))
                {
                    totalDistanceToNextPoint = 0.0001f;
                }
            }
        }

        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            health = Mathf.Clamp(health, 0, maxHealth);
            Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + health);

            if (healthbar != null)
            {
                healthbar.UpdateHealth(health / maxHealth);
            }

            ShowDamageNumber(damageAmount);

            if (health <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log(gameObject.name + " has been destroyed!");

            FinanceManager financeManager = FindObjectOfType<FinanceManager>();
            if (financeManager != null)
            {
                financeManager.AddMoney();
            }
            else
            {
                Debug.LogWarning("FinanceManager not found in the scene!");
            }

            if (healthbar != null)
            {
                Destroy(healthbar.gameObject);
            }

            Destroy(gameObject);
        }

        private void ShowDamageNumber(float damageAmount)
        {
            if (damageNumberPrefab != null)
            {
                GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
                DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
                if (damageNumberScript != null)
                {
                    damageNumberScript.SetDamage(damageAmount);
                }
            }
        }
    }
}
