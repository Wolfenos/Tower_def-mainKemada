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
        private Vector3 startPosition;
        private float lerpProgress = 0f;
        private float totalDistanceToNextPoint = 0f;

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

                // We assume pathPoints[0] exists and is the first point to move to
                if (pathPoints.Length > 0)
                {
                    startPosition = transform.position;
                    SetNewTarget(pathPoints[0]); // Move towards the first path point
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
            if (currentPointIndex >= pathPoints.Length)
            {
                return;
            }

            Transform targetPoint = pathPoints[currentPointIndex];
            if (targetPoint == null)
            {
                return;
            }

            // Increase lerpProgress based on speed and distance
            float step = (moveSpeed * currentSpeedModifier * Time.deltaTime) / totalDistanceToNextPoint;
            lerpProgress += step;

            // Calculate the new horizontal position using Lerp
            Vector3 lerpPos = Vector3.Lerp(startPosition, targetPoint.position, lerpProgress);

            // Now adjust this position to the ground level using a raycast
            Vector3 adjustedPos = AdjustToGround(lerpPos);

            transform.position = adjustedPos;

            // Check if we reached the end of this segment
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

        private Vector3 AdjustToGround(Vector3 position)
        {
            float raycastHeightOffset = 10f;
            Vector3 rayOrigin = new Vector3(position.x, position.y + raycastHeightOffset, position.z);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity))
            {
                // If ground is hit, set Y to the ground level
                position.y = hit.point.y;
            }
            else
            {
                // If no ground is hit, log a warning and keep position.y as is
                Debug.LogWarning("No ground detected below the enemy. Check ground colliders.");
            }

            return position;
        }


        private void SetNewTarget(Transform nextPoint)
        {
            if (nextPoint != null)
            {
                totalDistanceToNextPoint = Vector3.Distance(startPosition, nextPoint.position);
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
