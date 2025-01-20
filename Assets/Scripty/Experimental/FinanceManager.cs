using UnityEngine;
using TMPro;

namespace KemadaTD
{
    public class FinanceManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI moneyText; // Reference to TextMeshProUGUI for displaying money

        [Header("Settings")]
        [SerializeField] private int startingMoney = 50;   // Starting amount of money
        [SerializeField] private int moneyPerKill = 1;     // Money earned per kill

        [Header("Toggle Settings")]
        [SerializeField] private bool useMoneyPerSecond = false; // Checkbox for money-per-second
        [SerializeField] private bool useMoneyPerKill = true;    // Checkbox for money-per-kill

        [Header("Money Per Second Settings")]
        [SerializeField] private float moneyPerSecond = 1f;  // Amount of money earned each second

        private float timer = 0f;    // Internal timer for money-per-second logic
        private int currentMoney;

        private void Start()
        {
            currentMoney = startingMoney;
            UpdateMoneyUI();
        }

        private void Update()
        {
            // If "Money Per Second" is enabled, accumulate money over time
            if (useMoneyPerSecond)
            {
                timer += Time.deltaTime;

                // Every 1 second, add money
                if (timer >= 1f)
                {
                    int moneyToAdd = Mathf.RoundToInt(moneyPerSecond);
                    currentMoney += moneyToAdd;
                    UpdateMoneyUI();
                    timer = 0f;
                }
            }
        }

        // Method to add money on enemy kill
        public void AddMoney()
        {
            if (useMoneyPerKill)
            {
                currentMoney += moneyPerKill;
                UpdateMoneyUI();
            }
        }

        // Overload: Add a custom amount of money
        public void AddMoney(int amount)
        {
            currentMoney += amount;
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
