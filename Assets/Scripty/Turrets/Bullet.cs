using UnityEngine;

namespace KemadaTD
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 70f; // Rychlost projektilu
        private Transform target; // Cíl, na který střela míří
        private float damage;     // Poškození, které střela způsobí

        // Inicializuje střelu s cílem a poškozením
        public void Seek(Transform target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private void Update()
        {
            // Pokud není cíl, střela se zničí
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            // Pohybuje se směrem k cíli
            Vector3 direction = target.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            // Kontrola, zda střela zasáhla cíl
            if (direction.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        }

        // Po zásahu aplikuje poškození a zničí střelu
        private void HitTarget()
        {
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject); // Zničí střelu po zásahu
        }
    }
}