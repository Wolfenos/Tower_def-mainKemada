using UnityEngine;
using TMPro;

namespace KemadaTD
{
    public class FinanceManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI moneyText; // Reference to TextMeshProUGUI for displaying money

        [Header("Settings")]
        [SerializeField] private int startingMoney = 50; // Starting amount of money, adjustable in Inspector
        [SerializeField] private int moneyPerKill = 1;   // Amount of money earned per enemy kill

        private int currentMoney;

        private void Start()
        {
            currentMoney = startingMoney; // Set the initial money to startingMoney
            UpdateMoneyUI();
        }

        // Method to add money when an enemy is killed
        public void AddMoney()
        {
            currentMoney += moneyPerKill;
            UpdateMoneyUI();
        }

        // Method to deduct money, used for purchases; ensures balance doesn’t go negative
        public bool SpendMoney(int amount)
        {
            if (currentMoney >= amount)
            {
                currentMoney -= amount;
                UpdateMoneyUI();
                return true; // Successful purchase
            }
            else
            {
                Debug.Log("Not enough money to complete the transaction.");
                return false; // Failed purchase
            }
        }

        // Method to update the money displayed on the UI
        private void UpdateMoneyUI()
        {
            if (moneyText != null)
            {
                moneyText.text = "Money: " + currentMoney;
            }
            else
            {
                Debug.LogWarning("Money Text UI (TextMeshPro) is not assigned in the Inspector!");
            }
        }

        // Method to adjust money per kill (could be called by other scripts if needed)
        public void SetMoneyPerKill(int amount)
        {
            moneyPerKill = amount;
        }

        // Method to retrieve the current money value
        public int GetCurrentMoney()
        {
            return currentMoney;
        }
    }
}
