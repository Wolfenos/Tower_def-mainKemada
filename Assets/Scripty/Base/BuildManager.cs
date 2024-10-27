using UnityEngine;

namespace KemadaTD
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        private GameObject selectedTurretPrefab;
        private int selectedTurretCost;
        [SerializeField] private FinanceManager financeManager; // Reference to FinanceManager

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetTurretToBuild(GameObject turretPrefab, int turretCost)
        {
            selectedTurretPrefab = turretPrefab;
            selectedTurretCost = turretCost;
        }

        public void BuildTurret(Transform ringTransform)
        {
            if (selectedTurretPrefab == null)
            {
                Debug.LogWarning("No turret selected for building.");
                return;
            }

            if (financeManager.SpendMoney(selectedTurretCost))
            {
                GameObject turretInstance = Instantiate(selectedTurretPrefab, ringTransform.position, Quaternion.identity);
                turretInstance.transform.SetParent(ringTransform); // Set turret as child of ring
                Debug.Log("Turret built on ring!");
            }
            else
            {
                Debug.Log("Not enough money to build this turret.");
            }
        }
    }
}
