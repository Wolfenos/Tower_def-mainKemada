using UnityEngine;

namespace KemadaTD
{
    public class GridCell : MonoBehaviour
    {
        private GameObject currentTurret; // Holds a reference to the turret placed on this cell, if any

        // Checks if the cell is currently empty
        public bool IsEmpty()
        {
            return currentTurret == null;
        }

        // Sets a turret on this cell and marks it as occupied
        public void SetTurret(GameObject turret)
        {
            if (IsEmpty())
            {
                currentTurret = turret;
            }
            else
            {
                Debug.LogWarning("Attempted to place a turret on an occupied cell.");
            }
        }

        // Optional: Method to remove the turret from this cell, if needed
        public void ClearTurret()
        {
            if (currentTurret != null)
            {
                Destroy(currentTurret);
                currentTurret = null;
            }
        }

        // Visual feedback for selecting or deselecting the cell (e.g., highlighting)
        public void HighlightCell(bool isHighlighted)
        {
            // Implement a visual effect like changing the material or color when highlighted
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = isHighlighted ? Color.green : Color.white; // Example highlight color
            }
        }
    }
}
