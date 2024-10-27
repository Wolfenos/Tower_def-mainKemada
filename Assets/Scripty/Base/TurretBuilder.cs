using System.Collections;
using UnityEngine;

namespace KemadaTD
{
    public class TurretBuilder : MonoBehaviour
    {
        [Header("Ghost Turret")]
        public GameObject ghostTurretPrefab;  // The prefab for the ghost version of the turret
        private GameObject ghostTurretInstance;

        private GridCell selectedGridCell;
        private TurretManager turretManager;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            turretManager = GetComponent<TurretManager>();
            SpawnGhostTurret();
        }

        private void Update()
        {
            UpdateGhostTurretPosition();
            if (Input.GetMouseButtonDown(0) && selectedGridCell != null && selectedGridCell.IsEmpty())
            {
                int turretIndex = 0; // Assign the index you want to build, or pass it dynamically as needed
                BuildTurret(selectedGridCell, turretManager.GetTurretPrefab(turretIndex));
            }
        }


        private void SpawnGhostTurret()
        {
            // Instantiate a ghost turret at the start
            if (ghostTurretPrefab != null)
            {
                ghostTurretInstance = Instantiate(ghostTurretPrefab);
                ghostTurretInstance.SetActive(false); // Hide initially
            }
        }

        private void UpdateGhostTurretPosition()
        {
            if (ghostTurretInstance == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPosition = hit.point;

                // Snap to grid position based on your grid system
                selectedGridCell = GetGridCellAtPosition(targetPosition);
                if (selectedGridCell != null)
                {
                    ghostTurretInstance.SetActive(true);
                    bool isBuildable = selectedGridCell.IsEmpty();
                    ghostTurretInstance.GetComponent<GhostTurret>().UpdatePosition(selectedGridCell.transform.position, isBuildable);
                }
                else
                {
                    ghostTurretInstance.SetActive(false);
                }
            }
        }

        private GridCell GetGridCellAtPosition(Vector3 position)
        {
            // Implement your logic to get the closest grid cell at a specific position
            return /* code to get the closest grid cell */;
        }

        // Public method for TurretManager to call directly
        public void BuildTurret(GridCell gridCell, GameObject turretPrefab)
        {
            if (gridCell != null && gridCell.IsEmpty())
            {
                StartCoroutine(BuildTurretCoroutine(gridCell, turretPrefab));
                selectedGridCell = null;  // Clear selection after building
            }
        }

        private IEnumerator BuildTurretCoroutine(GridCell gridCell, GameObject turretPrefab)
        {
            yield return new WaitForSeconds(0.5f);  // Optional: slight delay

            GameObject newTurret = Instantiate(turretPrefab, gridCell.transform.position, Quaternion.identity);
            gridCell.SetTurret(newTurret);

            ghostTurretInstance.SetActive(false);  // Hide ghost after placing
        }
    }
}
