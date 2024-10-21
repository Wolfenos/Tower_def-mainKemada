using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class EnemyHealth : MonoBehaviour
    {
        public float maxHealth = 100f;  // Maximální zdraví nepřítele
        private float currentHealth;    // Aktuální zdraví nepřítele

        private void Start()
        {
            // Nastavení zdraví na maximální hodnotu na začátku
            currentHealth = maxHealth;
        }

        // Metoda pro snížení zdraví nepřítele
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;

            // Pokud zdraví klesne na nulu nebo méně, nepřítel je zničen
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Metoda pro zničení nepřítele
        private void Die()
        {
            Destroy(gameObject); // Zničí objekt nepřítele
        }
    }
}