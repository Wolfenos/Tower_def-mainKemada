using UnityEngine;

namespace KemadaTD
{
    public class Turret : MonoBehaviour
    {
        public float range = 10f;             // Dosah věže
        public float fireRate = 1f;           // Frekvence střelby (1 střela za sekundu)
        public float damage = 20f;            // Poškození způsobené jedním výstřelem
        public Transform partToRotate;        // Část věže, která se otáčí k cíli
        public GameObject bulletPrefab;       // Prefab projektilu
        public Transform firePoint;           // Místo, odkud se střely vystřelují

        private Transform target;             // Aktuální cíl věže
        private float fireCountdown = 0f;     // Odpočet pro další střelu

        private void Update()
        {
            UpdateTarget();

            if (target == null)
                return;

            // Otáčení věže k cíli
            Vector3 direction = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * 5f).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            // Střelba
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }

        // Hledá cíl v dosahu
        private void UpdateTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

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
                target = nearestEnemy.transform;
            }
            else
            {
                target = null;
            }
        }

        // Metoda pro střelbu na cíl
        private void Shoot()
        {
            GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.Seek(target, damage);
            }
        }

        // Zobrazuje dosah věže v editoru
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}