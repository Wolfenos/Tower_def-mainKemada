using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class Healthbarv2 : MonoBehaviour
    {
        public Slider healthSlider;
        public Slider easeHealthSlider;
        public float maxHealth = 100f;
        private float lerpspeed = 0.1f; // Increased lerpspeed for faster updates

        void Start()
        {
            healthSlider.value = maxHealth;
            easeHealthSlider.value = maxHealth;
        }

        public void UpdateHealth(float health)
        {
            health = Mathf.Clamp01(health); // Ensure health value is clamped between 0 and 1
            healthSlider.value = health;
        }

        void Update()
        {
            if (healthSlider.value != easeHealthSlider.value)
            {
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpspeed);
            }
        }
    }
}
