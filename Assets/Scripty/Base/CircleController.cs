using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor; // Needed for Handles
#endif

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

        [System.Serializable]
        public class Sector
        {
            public string name;           // Name of the sector (for identification)
            [Range(0f, 360f)]
            public float startAngle;      // Start angle in degrees
            [Range(0f, 360f)]
            public float endAngle;        // End angle in degrees
            public bool isActive;         // Active checkbox
            public bool isPassive;        // Passive checkbox
            public Color color = Color.green; // Color for visualization
        }

        public List<Sector> sectors = new List<Sector>(); // List of sectors

        public CircleInput inputSettings;  // Input settings for selecting and rotating circles
        public Transform[] circles;  // Array of circles to control
        public float rotationAmount = 90f;  // How much to rotate per input (e.g., 90 degrees)
        public float rotationDuration = 0.5f; // Duration for the rotation animation
        public float rotationSpeed = 1.0f; // Speed of the rotation animation, editable in the inspector

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
            if (circle == null || sectors == null || sectors.Count == 0)
                return;

            float radius = 5f; // Adjust as needed for visualization

#if UNITY_EDITOR
            // Save the current color
            Color oldColor = Handles.color;

            foreach (Sector sector in sectors)
            {
                // Set the color based on the sector's active/passive status
                Handles.color = sector.color;

                // Calculate the angles for the sector
                float startAngle = sector.startAngle;
                float endAngle = sector.endAngle;

                // Ensure startAngle is less than endAngle
                if (endAngle < startAngle)
                {
                    endAngle += 360f;
                }

                // Draw the sector as a filled arc
                Handles.DrawSolidArc(
                    circle.position,
                    Vector3.up,
                    Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward,
                    endAngle - startAngle,
                    radius
                );

                // Optionally, draw sector boundaries
                Handles.color = Color.black;
                Vector3 from = Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius;
                Vector3 to = Quaternion.Euler(0f, endAngle, 0f) * Vector3.forward * radius;
                Handles.DrawLine(circle.position, circle.position + from);
                Handles.DrawLine(circle.position, circle.position + to);
            }

            // Restore the old color
            Handles.color = oldColor;
#endif
        }
    }
}
