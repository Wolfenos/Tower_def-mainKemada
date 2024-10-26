using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class HealthBar : MonoBehaviour
    {
        public Slider healthSlider;         // Immediate health bar slider
        public Slider easeHealthSlider;     // Smooth health bar slider
        public float lerpSpeed = 0.05f;     // Speed for easing effect
        public Vector3 healthBarOffset = new Vector3(0, 2, 0); // Position offset for the health bar

        private float maxHealth;
        private float currentHealth;
        private Transform enemyTransform;

        // Initialize health and set up the health bar's association with an enemy
        public void Initialize(float maxHealth, Transform enemy)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            enemyTransform = enemy;

            UpdateHealthBarInstant();  // Set initial health bar value
        }

        // Method for applying damage
        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            UpdateHealthBarInstant();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Update()
        {
            // Smoothly update the easeHealthSlider to match healthSlider
            if (easeHealthSlider != null && easeHealthSlider.value != healthSlider.value)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed);
            }

            // Update health bar position to stay above the enemy
            if (enemyTransform != null)
            {
                transform.position = enemyTransform.position + healthBarOffset;
                transform.LookAt(Camera.main.transform);
            }
        }

        // Method to update health bar sliders immediately
        private void UpdateHealthBarInstant()
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth / maxHealth;
            }
        }

        // Handles enemy death
        private void Die()
        {
            // Destroy both the health bar and the enemy when health is zero
            Destroy(enemyTransform.gameObject);  // Destroy the enemy
            Destroy(gameObject);                 // Destroy the health bar
        }
    }
}
