using UnityEngine;

namespace KemadaTD
{
    public class GridCell : MonoBehaviour
    {
        private GameObject turret; // The turret placed on this grid cell
        private TurretBuilder turretBuilder; // Reference to the TurretBuilder

        private void Start()
        {
            // Find the TurretBuilder in the scene (you could assign this manually in the Inspector as well)
            turretBuilder = FindObjectOfType<TurretBuilder>();
        }

        public bool IsEmpty()
        {
            return turret == null;
        }

        public void SetTurret(GameObject newTurret)
        {
            turret = newTurret;
        }

        public GameObject GetTurret()
        {
            return turret;
        }

        public bool HasTurret() // This method returns true if a turret is present
        {
            return turret != null;
        }

        // Detect when the player clicks on the grid cell
        private void OnMouseDown()
        {
            if (IsEmpty())
            {
                turretBuilder.ShowBuildMenu(this); // Pass this grid cell to the TurretBuilder for the menu
            }
        }
    }
}
