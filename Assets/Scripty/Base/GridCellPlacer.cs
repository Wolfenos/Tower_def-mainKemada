using UnityEngine;

namespace KemadaTD
{
    public class GridCellPlacer : MonoBehaviour
    {
        public GameObject gridCellPrefab; // Assign this in the inspector
        public float circleRadius = 5f; // Adjust based on your circle size
        public int numberOfSectors = 4; // Fixed at 4 sectors (like a pizza)
        public int gridCellsPerSector = 2; // Set to 2 grid cells per sector

        void Start()
        {
            PlaceGridCells();
        }

        // Method to place the grid cells evenly in each sector
        void PlaceGridCells()
        {
            // Angle step for each sector
            float angleStep = 360f / numberOfSectors;

            // Loop through each sector
            for (int sector = 0; sector < numberOfSectors; sector++)
            {
                // Place two grid cells in each sector
                for (int i = 0; i < gridCellsPerSector; i++)
                {
                    // Calculate the angle for grid cells within this sector
                    // We distribute the cells slightly apart within each sector
                    float angle = (sector * angleStep) + (angleStep / (gridCellsPerSector + 1) * (i + 1));

                    // Get the position of the grid cell on the circle based on the angle
                    Vector3 cellPosition = GetPositionOnCircle(angle);

                    // Instantiate the grid cell prefab at the calculated position
                    Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                }
            }
        }

        // Helper method to calculate a grid cell's position on the circle based on an angle
        Vector3 GetPositionOnCircle(float angle)
        {
            // Convert angle from degrees to radians
            float radians = angle * Mathf.Deg2Rad;

            // Calculate the x and z coordinates based on the circle's radius
            float x = Mathf.Cos(radians) * circleRadius;
            float z = Mathf.Sin(radians) * circleRadius;

            // Return the calculated position, keeping y the same (assuming the circle is flat on the ground)
            return new Vector3(x, 0, z) + transform.position;
        }
    }
}
