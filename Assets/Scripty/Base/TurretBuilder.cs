using UnityEngine;

namespace KemadaTD
{
    public class TurretBuilder : MonoBehaviour
    {
        // Reference to the grid cells on the circle
        public GridCell[] gridCells;

        // Turret prefab to instantiate
        public GameObject turretPrefab;

        // Try to place a turret at the specified grid position
        public void TryPlaceTurret(Vector3 gridPosition)
        {
            GridCell cell = GetGridCellAtPosition(gridPosition);
            if (cell != null && cell.IsEmpty())
            {
                PlaceTurret(cell);
            }
        }

        // Place a turret at a specific grid cell
        private void PlaceTurret(GridCell cell)
        {
            GameObject newTurret = Instantiate(turretPrefab, cell.transform.position, Quaternion.identity);
            cell.SetTurret(newTurret); // Mark the cell as occupied
        }

        // Find the grid cell at a specific position
        private GridCell GetGridCellAtPosition(Vector3 position)
        {
            // Assuming you use a method like a raycast or simple 2D grid mapping
            foreach (GridCell cell in gridCells)
            {
                if (cell.ContainsPosition(position))
                {
                    return cell;
                }
            }
            return null;
        }
    }
}
