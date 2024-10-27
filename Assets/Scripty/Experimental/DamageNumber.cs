using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class DamageNumber : MonoBehaviour
    {
        public Text damageText;
        public float floatSpeed = 1f;
        public float fadeSpeed = 1f;

        public void SetDamage(float damageAmount)
        {
            damageText.text = damageAmount.ToString();
        }

        private void Update()
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            Color color = damageText.color;
            color.a -= fadeSpeed * Time.deltaTime;
            damageText.color = color;

            if (color.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

