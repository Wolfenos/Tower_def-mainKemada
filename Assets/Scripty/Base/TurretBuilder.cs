using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class TurretBuilder : MonoBehaviour
    {
        // Turret prefabs that can be selected (assigned in the Inspector)
        public GameObject[] turretPrefabs;
        public int maxTurretsInMenu = 3; // Control how many turrets can appear in the build menu

        // UI elements for building
        public GameObject buildMenuUI; // The UI panel for the turret selection menu
        public Button[] turretButtons; // Buttons for selecting turrets
        public float buildTime = 3f; // Time it takes to build a turret (editable in the Inspector)

        private GridCell selectedGridCell; // The currently selected grid cell

        private void Start()
        {
            // Initially hide the build menu
            buildMenuUI.SetActive(false);
        }

        // Show the build menu when a grid cell is clicked
        public void ShowBuildMenu(GridCell gridCell)
        {
            selectedGridCell = gridCell;

            // Enable only the buttons for available turrets
            for (int i = 0; i < turretButtons.Length; i++)
            {
                if (i < turretPrefabs.Length && i < maxTurretsInMenu)
                {
                    turretButtons[i].gameObject.SetActive(true);
                    int turretIndex = i; // Capture the correct index for the turret
                    turretButtons[i].onClick.RemoveAllListeners(); // Clear any existing listeners
                    turretButtons[i].onClick.AddListener(() => StartBuildingTurret(turretIndex));
                }
                else
                {
                    turretButtons[i].gameObject.SetActive(false); // Disable unused buttons
                }
            }

            // Show the menu UI
            buildMenuUI.SetActive(true);
        }

        // Start the process of building a turret
        private void StartBuildingTurret(int turretIndex)
        {
            if (selectedGridCell != null && selectedGridCell.IsEmpty())
            {
                // Hide the build menu
                buildMenuUI.SetActive(false);

                // Start coroutine to build the turret
                StartCoroutine(BuildTurret(selectedGridCell, turretPrefabs[turretIndex]));
            }
        }

        // Coroutine for building a turret over time
        private IEnumerator BuildTurret(GridCell gridCell, GameObject turretPrefab)
        {
            // Simulate building time (e.g., wait for buildTime seconds)
            yield return new WaitForSeconds(buildTime);

            // Instantiate the turret on the grid cell after the build time
            GameObject newTurret = Instantiate(turretPrefab, gridCell.transform.position, Quaternion.identity);
            gridCell.SetTurret(newTurret);
        }
    }
}
