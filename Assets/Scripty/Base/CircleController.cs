using UnityEngine;

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        // Array of circle transforms (make sure to assign them in the Inspector)
        public Transform[] circles;

        // Degrees to rotate for each key press (e.g., 90 degrees)
        public float rotationAmount = 90f;

        // Active sectors (an array indicating which sectors are active for each circle)
        public int[] activeSectors; // Example: 3 for active sectors, 1 for passive

        // Update is called once per frame
        void Update()
        {
            // Rotate first circle (index 0) using arrow keys
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotateCircle(0, -rotationAmount); // Rotate left
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotateCircle(0, rotationAmount); // Rotate right
            }

            // You can extend this for other circles with other keys
            // e.g., A/D for the second circle, Q/E for the third circle
        }

        // Rotate a specific circle by the given degrees
        void RotateCircle(int circleIndex, float degrees)
        {
            if (!IsSectorPassive(circleIndex))
            {
                // Apply the rotation to the circle's transform
                circles[circleIndex].Rotate(0, degrees, 0);
            }
        }

        // Check if the current sector is passive
        bool IsSectorPassive(int circleIndex)
        {
            // Simple check based on activeSectors array (can be made more complex)
            // Assume each circle has 4 sectors, we divide 360 by 4 to get 90-degree sectors
            int sectorIndex = Mathf.FloorToInt(circles[circleIndex].rotation.eulerAngles.y / 90) % 4;

            // If the sectorIndex is 3 (i.e., the passive sector), disallow rotation
            return sectorIndex == 3;
        }
    }
}
