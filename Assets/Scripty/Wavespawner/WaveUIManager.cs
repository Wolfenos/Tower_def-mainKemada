using UnityEngine;
using TMPro;
using System.Collections;

namespace KemadaTD
{
    public class WaveUIEnhancer : MonoBehaviour
    {
        // Simple singleton so WaveManager can reference .Instance for the countdown
        public static WaveUIEnhancer Instance { get; private set; }

        [Header("References")]
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private FinanceManager financeManager;

        [Header("UI - Enemies Info")]
        [SerializeField] private TextMeshProUGUI enemyCountText;
        // "Living: X | Soon: Y"

        [Header("UI - Wave Delay")]
        [SerializeField] private TextMeshProUGUI waveDelayText;
        // A text field to show countdown or message
        [SerializeField] private GameObject skipButton;
        // A button to skip the timer

        // Tracking enemy counts
        private int livingEnemies;
        private int toSpawnEnemies;

        // For wave-delay skipping
        private float currentDelay;
        private bool skipRequested;

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

        private void OnEnable()
        {
            // Subscribe to the waveManager's spawn event
            if (waveManager != null)
            {
                waveManager.OnEnemySpawned += HandleEnemySpawned;
            }

            // Subscribe to the Enemy OnEnemyDied event
            Enemy.OnEnemyDied += HandleEnemyDied;
        }

        private void OnDisable()
        {
            // Unsubscribe
            if (waveManager != null)
            {
                waveManager.OnEnemySpawned -= HandleEnemySpawned;
            }

            Enemy.OnEnemyDied -= HandleEnemyDied;
        }

        // Called by WaveManager when a new wave is about to start spawning
        // to let us know how many enemies will spawn in that wave.
        public void PrepareForNewWave(int totalEnemiesInWave)
        {
            livingEnemies = 0;
            toSpawnEnemies = totalEnemiesInWave;
            UpdateEnemyUIText();
        }

        private void HandleEnemySpawned(GameObject enemyObj)
        {
            // One more enemy is now "alive"
            livingEnemies++;
            // One less is "to spawn"
            toSpawnEnemies--;
            UpdateEnemyUIText();
        }

        private void HandleEnemyDied(Enemy enemy)
        {
            // One less living
            livingEnemies--;
            if (livingEnemies < 0) livingEnemies = 0;
            UpdateEnemyUIText();
        }

        private void UpdateEnemyUIText()
        {
            if (enemyCountText != null)
            {
                enemyCountText.text = $"Living: {livingEnemies} | Soon: {toSpawnEnemies}";
            }
        }

        // --------------- WAVE DELAY HANDLER ---------------

        public IEnumerator HandleWaveDelay(float delay)
        {
            skipRequested = false;
            currentDelay = delay;

            if (skipButton != null)
                skipButton.SetActive(true);

            while (currentDelay > 0f && !skipRequested)
            {
                if (waveDelayText != null)
                {
                    waveDelayText.text = $"Next Wave in {currentDelay:F1}s";
                }
                currentDelay -= Time.deltaTime;
                yield return null;
            }

            // If we skip, we might want to award skipping
            if (skipRequested)
            {
                float secondsSkipped = currentDelay;
                // e.g. 0.5 money per second skipped
                float moneyToAdd = 0.5f * secondsSkipped;
                if (financeManager != null && moneyToAdd > 0f)
                {
                    financeManager.AddMoney(Mathf.RoundToInt(moneyToAdd));
                }
            }

            // Done waiting
            currentDelay = 0f;

            if (skipButton != null)
                skipButton.SetActive(false);

            if (waveDelayText != null)
            {
                waveDelayText.text = "Wave Starting...";
            }

            yield return new WaitForSeconds(0.5f);
            // short pause for clarity, optional

            if (waveDelayText != null)
            {
                waveDelayText.text = "";
            }
        }

        // Called by the UI button to skip the delay
        public void OnSkipWaveButtonPressed()
        {
            skipRequested = true;
        }
    }
}
