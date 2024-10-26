using UnityEngine;

namespace KemadaTD
{
    public class GroundController : MonoBehaviour
    {
        [Tooltip("Modifier for enemy speed when on this ground. 1 = normal speed, <1 = slower, >1 = faster")]
        public float speedModifier = 1f;

        // Tag to identify ground type, e.g., "SlowGround", "NormalGround"
        public string groundTag = "NormalGround";

        private void Awake()
        {
            // Set the tag in case it hasn't been set manually
            gameObject.tag = groundTag;
        }
    }
}