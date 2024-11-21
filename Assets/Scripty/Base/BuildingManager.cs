// BuildingManager.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KemadaTD
{
    public class BuildingManager : MonoBehaviour
    {
        [Header("Tower Manager Reference")]
        [SerializeField] private TowerManager towerManager;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI buildText;

        private bool isBuilding = false;
        private Vector3 centerPoint;

        // Store references to all anchor points in the scene
        private List<AnchorHighlighter> anchors = new List<AnchorHighlighter>();

        private void Start()
        {
            // Find all anchor objects in the scene with AnchorHighlighter attached
            anchors.AddRange(FindObjectsOfType<AnchorHighlighter>());
            
            CircleController circleController = FindObjectOfType<CircleController>();
            if (circleController != null)
            {
                centerPoint = circleController.transform.position;
            }
            else
            {
                centerPoint = Vector3.zero; // Default to (0,0,0) if CircleController not found
                Debug.LogWarning("CircleController not found. Using Vector3.zero as center point.");
            }
        }

        private void Update()
        {
            if (isBuilding && Input.GetMouseButtonDown(0))
            {
                BuildTurret();
            }
        }

        public void StartBuilding()
        {
            if (towerManager.GetSelectedTurret() != null)
            {
                isBuilding = true;
                buildText.text = "Click on an Anchor to build the turret.";
                buildText.gameObject.SetActive(true);

                HighlightAvailableAnchors(true); // Enable highlight
            }
        }

        private void BuildTurret()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Anchor"))
                {
                    GameObject anchor = hit.collider.gameObject;
                    TurretData selectedTurret = towerManager.GetSelectedTurret();

                    if (selectedTurret != null && anchor.transform.childCount == 0)
                    {
                        if (towerManager.financeManager.SpendMoney(selectedTurret.cost))
                        {
                            // Calculate the direction from the center point to the anchor
                            Vector3 centerPoint = Vector3.zero; // Adjust if necessary (e.g., get from CircleController)
                            Vector3 direction = anchor.transform.position - centerPoint;

                            // Flatten the direction vector to the XZ plane
                            direction.y = 0f;

                            // Check for zero magnitude to avoid errors
                            if (direction.sqrMagnitude == 0f)
                            {
                                direction = anchor.transform.forward; // Default to anchor's forward if direction is zero
                            }
                            else
                            {
                                direction = direction.normalized;
                            }

                            // Create a rotation that faces along that direction
                            Quaternion rotation = Quaternion.LookRotation(direction);

                            // Instantiate the turret with the calculated rotation
                            GameObject turretInstance = Instantiate(selectedTurret.prefab, anchor.transform.position, rotation);
                            turretInstance.transform.SetParent(anchor.transform);
                            anchor.tag = "OccupiedAnchor";
                            isBuilding = false;
                            buildText.gameObject.SetActive(false);

                            HighlightAvailableAnchors(false); // Disable highlight
                        }
                        else
                        {
                            DisplayMessage("Not enough money to build this turret.");
                        }
                    }
                    else
                    {
                        DisplayMessage("This anchor is already occupied.");
                    }
                }
            }
        }


        private void HighlightAvailableAnchors(bool shouldHighlight)
        {
            foreach (var anchor in anchors)
            {
                if (anchor.gameObject.CompareTag("Anchor")) // Only highlight available anchors
                {
                    anchor.Highlight(shouldHighlight);
                }
            }
        }

        private void DisplayMessage(string message)
        {
            buildText.text = message;
            buildText.gameObject.SetActive(true);
            Invoke("ClearMessage", 2f); // Clear message after 2 seconds
        }

        private void ClearMessage()
        {
            buildText.gameObject.SetActive(false);
        }
    }
}
