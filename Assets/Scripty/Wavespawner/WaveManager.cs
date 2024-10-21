using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<WaveConfig> levelWaves; // List of waves for this level
        [SerializeField] private bool loopWaves = false;      // Option to loop waves continuously

        private int currentWaveIndex = 0;

        private void Start()
        {
            StartCoroutine(SpawnAllWaves());
        }

        // Coroutine for spawning all waves in the level
        private IEnumerator SpawnAllWaves()
        {
            do
            {
                for (int i = currentWaveIndex; i < levelWaves.Count; i++)
                {
                    yield return StartCoroutine(SpawnWave(levelWaves[i]));
                    yield return new WaitForSeconds(levelWaves[i].waveDelay); // Delay between waves
                }
            } while (loopWaves);
        }

        // Coroutine for spawning all PathWaves in a single wave
        private IEnumerator SpawnWave(WaveConfig waveConfig)
        {
            List<Coroutine> pathWaveCoroutines = new List<Coroutine>();

            foreach (var pathWave in waveConfig.pathWaves)
            {
                if (pathWave.enemyPath != null)
                {
                    pathWaveCoroutines.Add(StartCoroutine(SpawnEnemiesForPath(pathWave)));
                }
            }

            // Wait for all path wave coroutines to complete
            foreach (Coroutine coroutine in pathWaveCoroutines)
            {
                yield return coroutine;
            }
        }

        // Coroutine for spawning enemies for a specific path wave
        private IEnumerator SpawnEnemiesForPath(WaveConfig.PathWave pathWave)
        {
            // Check if enemyPath or pathPoints are null
            if (pathWave.enemyPath == null)
            {
                Debug.LogError("Enemy path is not assigned in PathWave.");
                yield break;
            }

            if (pathWave.enemyPath.pathPoints == null || pathWave.enemyPath.pathPoints.Count == 0)
            {
                Debug.LogError("Path points are not assigned correctly in EnemyPath.");
                yield break;
            }

            for (int j = 0; j < pathWave.enemyTypes.Count; j++)
            {
                GameObject enemyType = pathWave.enemyTypes[j];

                // Check if enemyType is null
                if (enemyType == null)
                {
                    Debug.LogError("Enemy type is missing in PathWave.");
                    continue;  // Skip this enemy type and move to the next one
                }

                // Check if enemyCounts list matches enemyTypes list
                if (j >= pathWave.enemyCounts.Count)
                {
                    Debug.LogError("Mismatch between enemyTypes and enemyCounts in PathWave.");
                    yield break;
                }

                int count = pathWave.enemyCounts[j];

                // Spawn each enemy of this type
                for (int i = 0; i < count; i++)
                {
                    // Double-check pathPoints is not empty and instantiate at the start point
                    if (pathWave.enemyPath.pathPoints[0] != null)
                    {
                        // Spawn the enemy at the first point of the path
                        GameObject newEnemy = Instantiate(enemyType, pathWave.enemyPath.pathPoints[0].transform.position, Quaternion.identity);
                        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

                        if (enemyScript != null)
                        {
                            //Initialize the enemy with health and the path (you can specify health here, like 100f)
                            enemyScript.Initialize(100f, pathWave.enemyPath.transform);
                        }
                        else
                        {
                            Debug.LogError("Enemy prefab is missing the Enemy script component.");
                            Destroy(newEnemy);  // Destroy the instantiated enemy if it’s invalid
                        }
                    }
                    else
                    {
                        Debug.LogError("First path point is missing in EnemyPath.");
                    }

                    yield return new WaitForSeconds(pathWave.spawnInterval);
                }
            }
        }

    }
}



