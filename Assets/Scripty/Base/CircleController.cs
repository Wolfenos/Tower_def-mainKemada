using UnityEngine;

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        [System.Serializable]
        public class CircleInput
        {
            public Transform circle; // Reference to the circle object
            public Renderer renderer; // Renderer to change material for highlighting
            public Material defaultMaterial; // Default material
            public Material highlightMaterial; // Highlight material
        }

        public CircleInput[] circles; // Array of circles in the scene

        // Controls (configurable in the Inspector)
        public KeyCode switchRingKey = KeyCode.Tab; // Key to switch between rings
        public KeyCode rotateLeftKey = KeyCode.LeftArrow;  // Key to rotate selected ring left
        public KeyCode rotateRightKey = KeyCode.RightArrow; // Key to rotate selected ring right

        public float rotationAmount = 90f; // Rotation angle for each press (e.g., 90 degrees)

        private int selectedRingIndex = 0; // Tracks the currently selected ring

        private void Start()
        {
            // Highlight the initially selected ring
            HighlightSelectedRing();
        }

        private void Update()
        {
            HandleRingSwitching();
            HandleRingRotation();
        }

        // Handle switching between rings
        private void HandleRingSwitching()
        {
            if (Input.GetKeyDown(switchRingKey))
            {
                // Remove highlight from the currently selected ring
                UnhighlightRing(selectedRingIndex);

                // Cycle to the next ring
                selectedRingIndex = (selectedRingIndex + 1) % circles.Length;

                // Highlight the new selected ring
                HighlightSelectedRing();

                Debug.Log("Selected Ring: " + selectedRingIndex); // Optional: for debugging purposes
            }
        }

        // Handle rotation of the selected ring
        private void HandleRingRotation()
        {
            if (Input.GetKeyDown(rotateLeftKey))
            {
                RotateSelectedRing(-rotationAmount);
            }

            if (Input.GetKeyDown(rotateRightKey))
            {
                RotateSelectedRing(rotationAmount);
            }
        }

        // Rotate the currently selected ring by a specified amount
        private void RotateSelectedRing(float degrees)
        {
            Transform selectedCircle = circles[selectedRingIndex].circle;
            selectedCircle.Rotate(0, degrees, 0); // Rotate along the Y-axis
        }

        // Highlight the currently selected ring
        private void HighlightSelectedRing()
        {
            CircleInput selectedCircleInput = circles[selectedRingIndex];
            selectedCircleInput.renderer.material = selectedCircleInput.highlightMaterial;
        }

        // Unhighlight a ring based on its index
        private void UnhighlightRing(int ringIndex)
        {
            CircleInput circleInput = circles[ringIndex];
            circleInput.renderer.material = circleInput.defaultMaterial;
        }
    }
}
