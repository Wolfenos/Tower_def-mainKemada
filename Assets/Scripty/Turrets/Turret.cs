using UnityEngine;
using TMPro; // For TextMeshProUGUI

namespace KemadaTD
{
    public class Turret : MonoBehaviour
    {
        [Header("Turret Settings")]
        public float range = 10f;             // Range of the turret
        public float fireRate = 1f;           // Fire rate (shots per second)
        public float damage = 20f;            // Damage per shot

        [Header("Ammunition Settings")]
        public int maxAmmo = 10;              // Max ammo capacity
        public float reloadTime = 5f;         // Time to reload when in passive sector

        [Header("References")]
        public GameObject bulletPrefab;       // Prefab for the bullet
        public Transform[] firePoints;        // Array of points where the bullets are fired from
        public Transform rotatingPart;        // Part of the turret that will rotate towards the target
        public TextMeshProUGUI ammoText;      // UI Text to display ammo count (assign in Inspector)

        private int currentAmmo;              // Current ammo count
        private float reloadTimer = 0f;       // Timer for reloading
        private bool isReloading = false;     // Is the turret currently reloading

        private Transform target;             // Current target
        private float fireCountdown = 0f;     // Countdown for next shot

        // Sector logic variables
        private CircleController circleController; // Reference to the CircleController
        private Transform circle;                  // The circle (ring) this turret is on

        private void Start()
        {
            // Initialize ammo
            currentAmmo = maxAmmo;

            // Find the circle and CircleController
            FindCircleAndController();
        }

        private void Update()
        {
            // Update the current target
            UpdateTarget();

            // Update sector status
            bool inActiveSector = IsInActiveSector();

            if (isReloading)
            {
                reloadTimer -= Time.deltaTime;
                if (reloadTimer <= 0f)
                {
                    // Finish reloading
                    currentAmmo = maxAmmo;
                    isReloading = false;
                }
            }

            if (!inActiveSector)
            {
                // If in passive sector, start reloading if not already at max ammo
                if (!isReloading && currentAmmo < maxAmmo)
                {
                    isReloading = true;
                    reloadTimer = reloadTime;
                    Debug.Log($"{gameObject.name}: Entered passive sector. Starting reload.");
                }

                // Cannot shoot in passive sector
                return;
            }

            // If in active sector and not reloading
            if (target == null || isReloading)
                return;

            // Rotate turret towards the target
            RotateTowardsTarget();

            // Check fire cooldown
            if (fireCountdown <= 0f) // Shoot if cooldown is complete
            {
                if (currentAmmo > 0)
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
                ammoText.text = currentAmmo.ToString();
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

        // Update the target based on finding the nearest enemy in range
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

        // Find the circle and CircleController the turret is on
        private void FindCircleAndController()
        {
            Transform current = transform.parent;
            while (current != null)
            {
                CircleController cc = current.GetComponent<CircleController>();
                if (cc != null)
                {
                    circle = current;
                    circleController = cc;
                    break;
                }
                current = current.parent;
            }

            if (circleController == null)
            {
                Debug.LogWarning($"{gameObject.name}: CircleController not found in parent hierarchy.");
            }
            else
            {
                Debug.Log($"{gameObject.name}: Found CircleController on {circleController.gameObject.name}.");
            }
        }

        // Determine if the turret is in an active sector
        private bool IsInActiveSector()
        {
            if (circle == null || circleController == null)
            {
                Debug.LogWarning($"{gameObject.name}: Circle or CircleController is null.");
                return true; // Default to active if circle or CircleController not found
            }

            // Transform the turret's position into the circle's local space
            Vector3 localPos = circle.InverseTransformPoint(transform.position);

            // Calculate the angle between the local forward direction and the turret's local position
            float angle = Mathf.Atan2(localPos.x, localPos.z) * Mathf.Rad2Deg;
            if (angle < 0f)
                angle += 360f;

            // Debug: Output the turret's local position and calculated angle
            Debug.Log($"{gameObject.name}: Local Position: {localPos}, Angle: {angle}");

            foreach (var sector in circleController.sectors)
            {
                float startAngle = sector.startAngle;
                float endAngle = sector.endAngle;

                // Handle wrap-around
                if (endAngle < startAngle)
                    endAngle += 360f;

                // Check if the angle falls within the sector
                if (angle >= startAngle && angle <= endAngle)
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

                    // Debug: Output sector information
                    Debug.Log($"{gameObject.name}: In Sector '{sector.name}' (Start: {startAngle}, End: {endAngle}), IsActive: {sector.isActive}, IsPassive: {sector.isPassive}, IsActiveSector: {isActiveSector}");

                    return isActiveSector;
                }
            }

            // If not in any sector, default to passive
            Debug.Log($"{gameObject.name}: Not in any sector. Defaulting to passive.");
            return false;
        }

        // Visualize the turret range in the Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range); // Draw a red circle representing the turret's range
        }
    }
}
