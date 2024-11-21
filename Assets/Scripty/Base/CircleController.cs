using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor; // Needed for Handles
#endif

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        // Custom key bindings for controlling rings (set in Inspector)
        [System.Serializable]
        public class CircleInput
        {
            public KeyCode selectNextRingKey = KeyCode.Tab;  // Key to select the next ring
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

        public CircleInput inputSettings;  // Input settings for selecting and rotating rings
        public Transform[] rings;  // Array of rings to control
        public List<Sector> sectors = new List<Sector>(); // List of sectors
        public float rotationAmount = 90f;  // How much to rotate per input (e.g., 90 degrees)
        public float rotationDuration = 0.5f; // Duration for the rotation animation
        public float rotationSpeed = 1.0f; // Speed of the rotation animation, editable in the inspector

        private int currentRingIndex = 0;  // Index of the currently selected ring
        private bool isRotating = false; // To check if a rotation is in progress

        private void Update()
        {
            // Change selection of the current ring
            if (Input.GetKeyDown(inputSettings.selectNextRingKey) && !isRotating)
            {
                currentRingIndex = (currentRingIndex + 1) % rings.Length;
            }

            // Rotate the currently selected ring based on input
            if (Input.GetKeyDown(inputSettings.rotateLeftKey) && !isRotating)
            {
                StartCoroutine(RotateRing(rings[currentRingIndex], -rotationAmount));
            }
            if (Input.GetKeyDown(inputSettings.rotateRightKey) && !isRotating)
            {
                StartCoroutine(RotateRing(rings[currentRingIndex], rotationAmount));
            }
        }

        // Rotate the ring by a given number of degrees with animation
        private IEnumerator RotateRing(Transform ring, float degrees)
        {
            isRotating = true;
            Quaternion startRotation = ring.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, degrees, 0);
            float elapsedTime = 0f;

            while (elapsedTime < rotationDuration)
            {
                ring.rotation = Quaternion.Slerp(startRotation, endRotation, (elapsedTime / rotationDuration) * rotationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ring.rotation = endRotation;
            isRotating = false;
        }

        // Draw sector boundaries in the Unity editor
        private void OnDrawGizmos()
        {
            // Visualize sectors in world space
            DrawSectors();
        }

        // Draw the sectors in world space using Gizmos
        private void DrawSectors()
        {
            if (sectors == null || sectors.Count == 0)
                return;

            float radius = 10f; // Adjust as needed for visualization
            Vector3 center = transform.position; // Center point for sectors

#if UNITY_EDITOR
            // Save the current color
            Color oldColor = Handles.color;

            foreach (Sector sector in sectors)
            {
                // Set the color based on the sector's color
                Handles.color = sector.color;

                // Calculate the angles for the sector
                float startAngle = sector.startAngle;
                float endAngle = sector.endAngle;

                // Ensure startAngle is less than endAngle
                if (endAngle < startAngle)
                {
                    endAngle += 360f;
                }

                // Draw the sector as a filled arc in world space
                Handles.DrawSolidArc(
                    center,
                    Vector3.up,
                    Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward,
                    endAngle - startAngle,
                    radius
                );

                // Optionally, draw sector boundaries
                Handles.color = Color.black;
                Vector3 from = Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius;
                Vector3 to = Quaternion.Euler(0f, endAngle, 0f) * Vector3.forward * radius;
                Handles.DrawLine(center, center + from);
                Handles.DrawLine(center, center + to);
            }

            // Restore the old color
            Handles.color = oldColor;
#endif
        }
    }
}
