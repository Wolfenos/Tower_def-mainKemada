using UnityEngine;

namespace KemadaTD
{
    public class Turret : MonoBehaviour
    {
        public float range = 10f;             // Range of the turret
        public float fireRate = 1f;           // Fire rate (shots per second)
        public float damage = 20f;            // Damage per shot
        public GameObject bulletPrefab;       // Prefab for the bullet
        public Transform[] firePoints;        // Array of points where the bullets are fired from
        public Transform rotatingPart;        // Part of the turret that will rotate towards the target
        public bool canShoot = true;          // Flag to check if turret can shoot

        private Transform target;             // Current target
        private float fireCountdown = 0f;     // Countdown for next shot

        private void Update()
        {
            // Update the current target
            UpdateTarget();

            // If no target or cannot shoot, return early
            if (target == null || !canShoot)
                return;

            // Rotate turret towards the target
            RotateTowardsTarget();

            // Check fire cooldown
            if (fireCountdown <= 0f) // Shoot if cooldown is complete
            {
                Shoot();
                fireCountdown = 1f / fireRate; // Reset fire cooldown
            }

            fireCountdown -= Time.deltaTime; // Decrease cooldown
        }

        private void RotateTowardsTarget()
        {
            if (target == null || rotatingPart == null)
                return;

            // Calculate the direction to the target
            Vector3 direction = target.position - rotatingPart.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(rotatingPart.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
            rotatingPart.rotation = Quaternion.Euler(0f, rotation.y, 0f); // Rotate only on the Y-axis
        }

        private void Shoot()
        {
            // Instantiate bullets from all fire points
            foreach (Transform firePoint in firePoints)
            {
                GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Bullet bullet = bulletGO.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.Seek(target, damage); // Pass target and damage to the bullet
                }
            }
        }

        // Method to update the target based on finding the nearest enemy in range
        private void UpdateTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Ensure enemies are tagged as "Enemy"
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            // Loop through all enemies to find the nearest one in range
            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy <= range)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform; // Set the target to the nearest enemy
            }
            else
            {
                target = null;
            }
        }

        // Method to visualize the turret range in the Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range); // Draw a red circle representing the turret's range
        }
    }
}