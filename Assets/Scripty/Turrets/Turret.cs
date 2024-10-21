using UnityEngine;

namespace KemadaTD
{
    public class Turret : MonoBehaviour
    {
        public float range = 10f;             // Range of the turret
        public float fireRate = 1f;           // Fire rate (shots per second)
        public float damage = 20f;            // Damage per shot
        public GameObject bulletPrefab;       // Prefab for the bullet
        public Transform firePoint;           // Where the bullets are fired from
        public bool canShoot = true;          // Flag to check if turret can shoot

        private Transform target;             // Current target
        private float fireCountdown = 0f;     // Countdown for next shot

        private TurretAmmo turretAmmo;        // Reference to TurretAmmo script

        private void Start()
        {
            // Get reference to the TurretAmmo component
            turretAmmo = GetComponent<TurretAmmo>();
        }

        private void Update()
        {
            // Update the current target
            UpdateTarget();

            // If no target or cannot shoot, return early
            if (target == null || !canShoot || turretAmmo == null)
                return;

            // Check fire cooldown and ammo
            if (fireCountdown <= 0f && turretAmmo.UseAmmo()) // Use ammo and only shoot if there is ammo
            {
                Shoot();
                fireCountdown = 1f / fireRate; // Reset fire cooldown
            }

            fireCountdown -= Time.deltaTime; // Decrease cooldown
        }

        private void Shoot()
        {
            // Instantiate the bullet and set its target
            GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Seek(target, damage); // Pass target and damage to the bullet
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

