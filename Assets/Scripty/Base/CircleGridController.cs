using UnityEngine;

namespace KemadaTD
{
    public class CircleGridController : MonoBehaviour
    {
        public Transform circleTransform; // The rotating circle

        // This method should be called whenever the circle rotates
        public void OnCircleRotated(float degrees)
        {
            // Find all grid cells that are children of the circle
            GridCell[] gridCells = circleTransform.GetComponentsInChildren<GridCell>();

            // Update turret positions based on the circle's new rotation
            foreach (GridCell cell in gridCells)
            {
                if (cell.HasTurret())
                {
                    UpdateTurretPosition(cell);
                }
            }
        }

        // Update the position of a turret when the circle rotates
        private void UpdateTurretPosition(GridCell cell)
        {
            // Calculate the new position based on the circle's rotation
            Vector3 newPosition = GetRotatedPosition(cell.transform.position);
            cell.GetTurret().transform.position = newPosition;
        }

        // Helper method to calculate the new rotated position of a turret
        private Vector3 GetRotatedPosition(Vector3 originalPosition)
        {
            Vector3 offset = originalPosition - circleTransform.position;
            float angle = circleTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            return circleTransform.position + rotation * offset;
        }
    }
}
