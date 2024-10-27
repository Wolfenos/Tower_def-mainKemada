using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KemadaTD
{
    public class TowerManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject buttonPrefab; // Button prefab to be instantiated in UI for each turret
        [SerializeField] private Transform buttonParent;  // Parent object for buttons in the UI

        [Header("Turret Prefabs")]
        [SerializeField] private List<TurretData> turrets;    // List of turret prefabs

        [Header("Finance Manager")]
        [SerializeField] internal FinanceManager financeManager;  // Reference to FinanceManager

        private TurretData selectedTurret;

        private void Start()
        {
            foreach (var turret in turrets)
            {
                GameObject button = Instantiate(buttonPrefab, buttonParent);
                button.GetComponentInChildren<TextMeshProUGUI>().text = turret.turretName + " ($" + turret.cost + ")";
                button.GetComponent<Button>().onClick.AddListener(() => SelectTurret(turret));
            }
        }

        private void SelectTurret(TurretData turret)
        {
            selectedTurret = turret;
        }

        public TurretData GetSelectedTurret()
        {
            return selectedTurret;
        }
    }

    [System.Serializable]
    public class TurretData
    {
        public string turretName;
        public GameObject prefab;
        public int cost;
    }

    public class BuildingManager : MonoBehaviour
    {
        [Header("Tower Manager Reference")]
        [SerializeField] private TowerManager towerManager; // Reference to TowerManager

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BuildTurret();
            }
        }

        private void BuildTurret()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("BuildRing"))
                {
                    GameObject ring = hit.collider.gameObject;
                    TurretData selectedTurret = towerManager.GetSelectedTurret();

                    if (selectedTurret != null && ring.transform.childCount == 0)
                    {
                        if (towerManager.financeManager.SpendMoney(selectedTurret.cost))
                        {
                            GameObject turretInstance = Instantiate(selectedTurret.prefab, ring.transform.position, Quaternion.identity);
                            turretInstance.transform.SetParent(ring.transform);
                        }
                        else
                        {
                            Debug.Log("Not enough money to build this turret.");
                        }
                    }
                }
            }
        }
    }
}
