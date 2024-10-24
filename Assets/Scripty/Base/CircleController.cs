using UnityEngine;

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        // Custom key bindings for each circle (set in Inspector)
        [System.Serializable]
        public class CircleInput
        {
            public Transform circle; // Reference to the circle object
            public KeyCode rotateLeftKey = KeyCode.LeftArrow;  // Key to rotate left
            public KeyCode rotateRightKey = KeyCode.RightArrow; // Key to rotate right
        }

        public CircleInput[] circles;  // Array of circles with their custom input settings
        public int numberOfSectors = 4;  // Number of sectors per circle (default is 4)
        public float rotationAmount = 90f;  // How much to rotate per input (e.g., 90 degrees)
        public Color activeSectorColor = Color.green; // Color for active sectors in the editor
        public Color passiveSectorColor = Color.red;  // Color for the passive sector

        private void Update()
        {
            // Iterate through each circle and apply rotation based on input
            foreach (CircleInput circleInput in circles)
            {
                if (Input.GetKeyDown(circleInput.rotateLeftKey))
                {
                    RotateCircle(circleInput.circle, -rotationAmount);
                }
                if (Input.GetKeyDown(circleInput.rotateRightKey))
                {
                    RotateCircle(circleInput.circle, rotationAmount);
                }
            }
        }

        // Rotate the circle by a given number of degrees
        private void RotateCircle(Transform circle, float degrees)
        {
            circle.Rotate(0, degrees, 0);  // Rotate along the Y-axis
        }

        // Draw sector boundaries in the Unity editor
        private void OnDrawGizmos()
        {
            // Visualize each circle's sectors
            foreach (CircleInput circleInput in circles)
            {
                DrawCircleSectors(circleInput.circle);
            }
        }

        // Draw the sectors for a specific circle using Gizmos
        private void DrawCircleSectors(Transform circle)
        {
            if (circle == null) return;

            // Calculate the angle step for each sector
            float sectorAngle = 360f / numberOfSectors;

            // World-aligned starting direction (forward, which is Z+ in Unity)
            Vector3 startingDirection = Vector3.forward;

            for (int i = 0; i < numberOfSectors; i++)
            {
                // Calculate the world-aligned rotation for this sector
                float currentAngle = i * sectorAngle;
                Quaternion sectorRotation = Quaternion.Euler(0, currentAngle, 0);
                Vector3 sectorDirection = sectorRotation * startingDirection;

                // Set the color based on whether the sector is passive (last sector) or active
                if (i == numberOfSectors - 1)
                    Gizmos.color = passiveSectorColor;  // Passive sector (e.g., sector 4)
                else
                    Gizmos.color = activeSectorColor;   // Active sectors

                // Draw the sector as a line starting from the circle's position
                Gizmos.DrawLine(circle.position, circle.position + sectorDirection * 5f);  // 5f is the length of the line for visualization
            }
        }
    }
}
