using KemadaTD;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace KemadaTD
{
    public class TowerManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button[] turretButtons; // Array of buttons to select specific turrets
        [SerializeField] private TurretData[] turrets;   // Array of turret prefabs, corresponding to the buttons

        [Header("Finance Manager")]
        [SerializeField] internal FinanceManager financeManager;  // Reference to FinanceManager

        private TurretData selectedTurret;

        private void Start()
        {
            for (int i = 0; i < turretButtons.Length; i++)
            {
                int index = i; // Cache index to use in lambda expression
                turretButtons[i].onClick.AddListener(() => SelectTurret(turrets[index]));
            }
        }

        private void SelectTurret(TurretData turret)
        {
            selectedTurret = turret;
            Debug.Log($"Selected turret: {turret.turretName}");
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
}