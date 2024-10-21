using UnityEngine;

namespace KemadaTD
{
    public class GridCell : MonoBehaviour
    {
        private GameObject turret;

        // Checks if this grid cell is empty
        public bool IsEmpty()
        {
            return turret == null;
        }

        // Sets the turret that occupies this grid cell
        public void SetTurret(GameObject newTurret)
        {
            turret = newTurret;
        }

        // Gets the turret occupying this grid cell (if any)
        public GameObject GetTurret()
        {
            return turret;
        }

        // Optionally, a method to check if a position is within this cell
        public bool ContainsPosition(Vector3 position)
        {
            return Vector3.Distance(position, transform.position) < 0.5f;
        }
    }
}
