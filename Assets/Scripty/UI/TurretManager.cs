using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KemadaTD
{
    [System.Serializable]
    public class TurretData
    {
        public GameObject turretPrefab;
        public int price;
    }

    public class TurretManager : MonoBehaviour
    {
        [Header("Turret Settings")]
        [SerializeField] private List<TurretData> turrets = new List<TurretData>();

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI[] turretPriceTexts; // Text components for displaying turret prices in the UI

        private FinanceManager financeManager;
        private TurretBuilder turretBuilder;

        private void Start()
        {
            financeManager = FindObjectOfType<FinanceManager>();
            turretBuilder = GetComponent<TurretBuilder>();

            UpdateTurretPriceUI();
        }

        // Method to get the count of available turrets
        public int GetTurretCount()
        {
            return turrets.Count;
        }

        // Method to update turret price UI for each turret
        private void UpdateTurretPriceUI()
        {
            for (int i = 0; i < turretPriceTexts.Length && i < turrets.Count; i++)
            {
                turretPriceTexts[i].text = "Price: " + turrets[i].price;
            }
        }

        // Method to attempt building a turret
        public bool TryBuildTurret(int turretIndex, GridCell selectedGridCell)
        {
            if (turretIndex < 0 || turretIndex >= turrets.Count) return false;

            TurretData selectedTurret = turrets[turretIndex];

            // Check if the player has enough money
            if (financeManager != null && financeManager.GetCurrentMoney() >= selectedTurret.price)
            {
                // Deduct money and initiate turret building
                if (financeManager.SpendMoney(selectedTurret.price))
                {
                    turretBuilder.BuildTurret(selectedGridCell, selectedTurret.turretPrefab);
                    return true;
                }
            }
            else
            {
                Debug.Log("Not enough money to build this turret.");
            }
            return false;
        }
        public GameObject GetTurretPrefab(int index)
        {
            if (index >= 0 && index < turrets.Count)
            {
                return turrets[index].turretPrefab;
            }
            return null;
        }

    }
}
