using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<WaveConfig> levelWaves;
        [SerializeField] private bool loopWaves = false;

        // Event to let WaveUIEnhancer track spawns
        public System.Action<GameObject> OnEnemySpawned;

        private int currentWaveIndex = 0;

        // Track living enemies in the current wave
        private int waveEnemiesToSpawn = 0;
        private int waveEnemiesAlive = 0;

        private void OnEnable()
        {
            // Subscribe to the Enemy death event
            Enemy.OnEnemyDied += OnEnemyDied;
        }

        private void OnDisable()
        {
            Enemy.OnEnemyDied -= OnEnemyDied;
        }

        private void Start()
        {
            StartCoroutine(SpawnAllWaves());
        }

        private void OnEnemyDied(Enemy enemy)
        {
            // An enemy belonging to this wave was destroyed
            waveEnemiesAlive = Mathf.Max(0, waveEnemiesAlive - 1);
        }

        private IEnumerator SpawnAllWaves()
        {
            do
            {
                for (int i = currentWaveIndex; i < levelWaves.Count; i++)
                {
                    yield return StartCoroutine(SpawnWave(levelWaves[i]));

                    // Wait until the wave is cleared
                    yield return new WaitUntil(() => waveEnemiesAlive <= 0);

                    // Now do the wave delay (unless you want no delay if wave is empty)
                    yield return StartCoroutine(WaitBetweenWaves(levelWaves[i].waveDelay));
                }
            }
            while (loopWaves);
        }

        private IEnumerator WaitBetweenWaves(float delay)
        {
            // We delegate to WaveUIEnhancer if it exists
            if (WaveUIEnhancer.Instance != null)
            {
                yield return StartCoroutine(WaveUIEnhancer.Instance.HandleWaveDelay(delay));
            }
            else
            {
                // fallback
                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator SpawnWave(WaveConfig waveConfig)
        {
            // Calculate total enemies for the wave
            waveEnemiesToSpawn = 0;
            waveEnemiesAlive = 0;

            foreach (var pathWave in waveConfig.pathWaves)
            {
                for (int j = 0; j < pathWave.enemyCounts.Count; j++)
                {
                    waveEnemiesToSpawn += pathWave.enemyCounts[j];
                }
            }

            // Update waveEnemiesAlive to that total (none have died yet)
            waveEnemiesAlive = waveEnemiesToSpawn;

            // Notify UI how many to spawn
            if (WaveUIEnhancer.Instance != null)
            {
                WaveUIEnhancer.Instance.PrepareForNewWave(waveEnemiesToSpawn);
            }

            List<Coroutine> pathWaveCoroutines = new List<Coroutine>();
            foreach (var pathWave in waveConfig.pathWaves)
            {
                if (pathWave.enemyPath != null)
                {
                    pathWaveCoroutines.Add(StartCoroutine(SpawnEnemiesForPath(pathWave)));
                }
            }

            // Wait for all path spawns to complete
            foreach (Coroutine coroutine in pathWaveCoroutines)
            {
                yield return coroutine;
            }
        }

        private IEnumerator SpawnEnemiesForPath(WaveConfig.PathWave pathWave)
        {
            if (pathWave.enemyPath == null)
            {
                Debug.LogError("Enemy path is not assigned in PathWave.");
                yield break;
            }

            if ((pathWave.enemyPath.pathPoints == null || pathWave.enemyPath.pathPoints.Count == 0))
            {
                Debug.LogError("Path points are not assigned or empty in EnemyPath.");
                yield break;
            }

            int minCount = Mathf.Min(pathWave.enemyTypes.Count, pathWave.enemyCounts.Count);
            // If mismatch, we only spawn up to minCount entries

            for (int j = 0; j < minCount; j++)
            {
                GameObject enemyType = pathWave.enemyTypes[j];
                int count = pathWave.enemyCounts[j];

                for (int i = 0; i < count; i++)
                {
                    Transform spawnPoint = pathWave.enemyPath.GetRandomSpawnPoint();
                    if (spawnPoint == null)
                    {
                        Debug.LogError("No spawn points found in EnemyPath. Cannot spawn enemy.");
                        yield break;
                    }

                    Vector3 spawnPosition = spawnPoint.position;
                    // Raycast down
                    if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                    {
                        spawnPosition = hit.point + Vector3.up * 1f;
                    }

                    // random offset
                    float xOffset = Random.Range(pathWave.xSpawnOffsetRange.x, pathWave.xSpawnOffsetRange.y);
                    float zOffset = Random.Range(pathWave.zSpawnOffsetRange.x, pathWave.zSpawnOffsetRange.y);
                    spawnPosition += new Vector3(xOffset, 0f, zOffset);

                    GameObject newEnemy = Instantiate(enemyType, spawnPosition, Quaternion.identity);
                    Enemy enemyScript = newEnemy.GetComponent<Enemy>();

                    if (enemyScript != null)
                    {
                        enemyScript.Initialize(100f, pathWave.enemyPath.transform);

                        // Notify UI that a new enemy has spawned
                        OnEnemySpawned?.Invoke(newEnemy);
                    }
                    else
                    {
                        Debug.LogError("Enemy prefab is missing the Enemy script component.");
                        Destroy(newEnemy);
                    }

                    yield return new WaitForSeconds(pathWave.spawnInterval);
                }
            }
        }
    }
}
