using UnityEngine;

namespace KemadaTD
{
    public class Bullet : MonoBehaviour
    {
        private Transform target;
        private float damage; // Bullet damage

        public float speed = 20f;

        // Method to set the bullet's target and damage
        public void Seek(Transform _target, float _damage)
        {
            target = _target;
            damage = _damage;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Move the bullet towards the target
            Vector3 direction = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            // Check if we reach the target
            if (direction.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            // Move the bullet closer to the target
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        }

        // When the bullet hits the target, apply damage
        private void HitTarget()
        {
            // Check if the target has the Enemy component
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Apply damage to the enemy
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy the bullet after it hits
        }
    }
}