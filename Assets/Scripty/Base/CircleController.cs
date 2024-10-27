using UnityEngine;
using System.Collections;

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        // Custom key bindings for controlling circles (set in Inspector)
        [System.Serializable]
        public class CircleInput
        {
            public KeyCode selectNextCircleKey = KeyCode.Tab;  // Key to select the next circle
            public KeyCode rotateLeftKey = KeyCode.LeftArrow;  // Key to rotate left
            public KeyCode rotateRightKey = KeyCode.RightArrow; // Key to rotate right
        }

        public CircleInput inputSettings;  // Input settings for selecting and rotating circles
        public Transform[] circles;  // Array of circles to control
        public int numberOfSectors = 4;  // Number of sectors per circle (default is 4)
        public float rotationAmount = 90f;  // How much to rotate per input (e.g., 90 degrees)
        public float rotationDuration = 0.5f; // Duration for the rotation animation
        public float rotationSpeed = 1.0f; // Speed of the rotation animation, editable in the inspector
        public Color activeSectorColor = Color.green; // Color for active sectors in the editor
        public Color passiveSectorColor = Color.red;  // Color for the passive sector

        private int currentCircleIndex = 0;  // Index of the currently selected circle
        private bool isRotating = false; // To check if a rotation is in progress

        private void Update()
        {
            // Change selection of the current circle
            if (Input.GetKeyDown(inputSettings.selectNextCircleKey) && !isRotating)
            {
                currentCircleIndex = (currentCircleIndex + 1) % circles.Length;
            }

            // Rotate the currently selected circle based on input
            if (Input.GetKeyDown(inputSettings.rotateLeftKey) && !isRotating)
            {
                StartCoroutine(RotateCircle(circles[currentCircleIndex], -rotationAmount));
            }
            if (Input.GetKeyDown(inputSettings.rotateRightKey) && !isRotating)
            {
                StartCoroutine(RotateCircle(circles[currentCircleIndex], rotationAmount));
            }
        }

        // Rotate the circle by a given number of degrees with animation
        private IEnumerator RotateCircle(Transform circle, float degrees)
        {
            isRotating = true;
            Quaternion startRotation = circle.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, degrees, 0);
            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                circle.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime / rotationDuration) * rotationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            circle.rotation = endRotation;
            isRotating = false;
        }

        // Draw sector boundaries in the Unity editor
        private void OnDrawGizmos()
        {
            // Visualize each circle's sectors
            foreach (Transform circle in circles)
            {
                DrawCircleSectors(circle);
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
