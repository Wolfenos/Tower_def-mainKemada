using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class Enemy : MonoBehaviour
    {
        public float speed = 3.0f;           // Speed of the enemy
        private List<Vector3> pathPositions; // Path positions the enemy follows
        private int currentTargetIndex = 0;  // Current target index in the path

        public void Initialize(EnemyPath enemyPath)
        {
            pathPositions = enemyPath.GetPathPositions();
            StartCoroutine(FollowPath());
        }

        private IEnumerator FollowPath()
        {
            while (currentTargetIndex < pathPositions.Count)
            {
                Vector3 targetPosition = pathPositions[currentTargetIndex];

                // Move towards the target position
                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    transform.position += direction * speed * Time.deltaTime;
                    yield return null;
                }

                // Move to the next target
                currentTargetIndex++;
            }

            // Destroy the enemy when it reaches the end
            Destroy(gameObject);
        }
    }
}