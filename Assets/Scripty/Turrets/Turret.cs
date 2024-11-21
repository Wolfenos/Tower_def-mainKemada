using UnityEngine;
using TMPro; // For TextMeshProUGUI

namespace KemadaTD
{
    public class Turret : MonoBehaviour
    {
        [Header("Turret Settings")]
        public float range = 10f;             // Maximum range of the turret
        public float fireRate = 1f;           // Fire rate (shots per second)
        public float damage = 20f;            // Damage per shot
        public float fieldOfViewAngle = 60f;  // Field of view angle for targeting (in degrees)

        [Header("Ammunition Settings")]
        public int maxAmmo = 10;              // Max ammo capacity
        public float reloadTime = 5f;         // Time (in seconds) to fully reload from 0 to max ammo

        [Header("References")]
        public GameObject bulletPrefab;       // Prefab for the bullet
        public Transform[] firePoints;        // Array of points where the bullets are fired from
        public Transform rotatingPart;        // Part of the turret that will rotate towards the target
        public TextMeshProUGUI ammoText;      // UI Text to display ammo count (assign in Inspector)

        private float currentAmmo;            // Current ammo count (using float for smooth reloading)
        private float reloadRate;             // Rate at which ammo is reloaded per second

        private Transform target;             // Current target
        private float fireCountdown = 0f;     // Countdown for next shot

        // Sector logic variables
        private CircleController circleController; // Reference to the CircleController
        private Vector3 centerPoint = Vector3.zero; // Center point for sector calculations

        private void Start()
        {
            // Initialize ammo
            currentAmmo = maxAmmo;

            // Calculate reload rate
            reloadRate = maxAmmo / reloadTime;

            // Find the CircleController
            FindCircleController();
        }

        private void Update()
        {
            // Update sector status
            bool inActiveSector = IsInActiveSector();

            if (!inActiveSector)
            {
                // In passive sector: stop targeting and shooting, and reload continuously
                target = null; // Stop targeting

                // Reload ammo continuously until maxAmmo is reached
                if (currentAmmo < maxAmmo)
                {
                    currentAmmo += reloadRate * Time.deltaTime;
                    if (currentAmmo > maxAmmo)
                        currentAmmo = maxAmmo;
                }

                // Update ammo text UI
                if (ammoText != null)
                {
                    ammoText.text = Mathf.FloorToInt(currentAmmo).ToString();
                }

                // Exit Update() here since turret does not target or shoot in passive sector
                return;
            }

            // Turret is in active sector
            // Proceed to update target and handle shooting

            // Update the current target
            UpdateTarget();

            // If there's no target, do nothing
            if (target == null)
                return;

            // Rotate turret towards the target
            RotateTowardsTarget();

            // Check fire cooldown
            if (fireCountdown <= 0f) // Shoot if cooldown is complete
            {
                if (currentAmmo >= 1)
                {
                    Shoot();
                    currentAmmo--;
                    fireCountdown = 1f / fireRate; // Reset fire cooldown
                }
                else
                {
                    // Out of ammo, cannot shoot
                    Debug.Log($"{gameObject.name}: Out of ammo.");
                }
            }

            fireCountdown -= Time.deltaTime; // Decrease cooldown

            // Update ammo text UI
            if (ammoText != null)
            {
                ammoText.text = Mathf.FloorToInt(currentAmmo).ToString();
            }
        }

        // Rotate the turret towards the target
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

        // Fire at the target
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

        // Update the target based on finding the nearest enemy in cone
        private void UpdateTarget()
        {
            if (circleController == null)
                return; // Ensure CircleController is available

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Ensure enemies are tagged as "Enemy"
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            // Loop through all enemies to find the nearest one in the cone
            foreach (GameObject enemy in enemies)
            {
                Vector3 dirToEnemy = enemy.transform.position - transform.position;
                float distanceToEnemy = dirToEnemy.magnitude;

                // Check if the enemy is within range
                if (distanceToEnemy <= range)
                {
                    // Check if the enemy is within the field of view angle
                    float angleToEnemy = Vector3.Angle(transform.forward, dirToEnemy);

                    if (angleToEnemy <= fieldOfViewAngle / 2f)
                    {
                        // Enemy is within the cone
                        if (distanceToEnemy < shortestDistance)
                        {
                            shortestDistance = distanceToEnemy;
                            nearestEnemy = enemy;
                        }
                    }
                }
            }

            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform; // Set the target to the nearest enemy in the cone
            }
            else
            {
                target = null;
            }
        }

        // Find the CircleController in the scene
        private void FindCircleController()
        {
            circleController = FindObjectOfType<CircleController>();
            if (circleController == null)
            {
                Debug.LogWarning($"{gameObject.name}: CircleController not found in the scene.");
            }
            else
            {
                // Assume the center point is at the CircleController's position
                centerPoint = circleController.transform.position;
            }
        }

        // Determine if the turret is in an active sector
        private bool IsInActiveSector()
        {
            if (circleController == null)
            {
                Debug.LogWarning($"{gameObject.name}: CircleController is null.");
                return true; // Default to active if CircleController not found
            }

            // Calculate the direction from the center point to the turret's position in world space
            Vector3 direction = transform.position - centerPoint;

            // Calculate the angle between the world's forward direction and the direction to the turret
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (angle < 0f)
                angle += 360f;

            foreach (var sector in circleController.sectors)
            {
                float startAngle = sector.startAngle % 360f;
                float endAngle = sector.endAngle % 360f;

                // Check if the angle falls within the sector
                if (IsAngleInSector(angle, startAngle, endAngle))
                {
                    // Determine sector status based on Active and Passive checkboxes
                    bool isActiveSector;

                    if (sector.isActive || (sector.isActive && sector.isPassive))
                    {
                        // Sector is active
                        isActiveSector = true;
                    }
                    else if (sector.isPassive && !sector.isActive)
                    {
                        // Sector is passive
                        isActiveSector = false;
                    }
                    else
                    {
                        // If neither is selected, default to passive
                        isActiveSector = false;
                    }

                    return isActiveSector;
                }
            }

            // If not in any sector, default to passive
            return false;
        }

        // Helper method to determine if an angle is within a sector, accounting for wrap-around
        private bool IsAngleInSector(float angle, float startAngle, float endAngle)
        {
            // Angles are already normalized in the calling method
            if (startAngle <= endAngle)
            {
                // Normal sector (no wrap-around)
                return angle >= startAngle && angle <= endAngle;
            }
            else
            {
                // Wrap-around sector
                return angle >= startAngle || angle <= endAngle;
            }
        }

        // Visualize the turret range and cone in the Editor
        private void OnDrawGizmosSelected()
        {
            // Draw the range sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);

            // Draw the field of view cone
            Gizmos.color = Color.yellow;
            DrawFieldOfView();
        }

        // Helper method to draw the field of view cone
        private void DrawFieldOfView()
        {
            if (rotatingPart == null)
                return;

            Vector3 forwardDirection = transform.forward;

            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0) * forwardDirection;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0) * forwardDirection;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * range);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * range);

            // Optionally, draw an arc to represent the cone (for visualization)
            int segments = 20;
            float angleStep = fieldOfViewAngle / segments;
            Vector3 previousPoint = transform.position + leftBoundary * range;

            for (int i = 1; i <= segments; i++)
            {
                float angle = -fieldOfViewAngle / 2f + angleStep * i;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * forwardDirection;
                Vector3 currentPoint = transform.position + dir * range;
                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }
    }
}
