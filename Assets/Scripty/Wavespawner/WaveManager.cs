using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<WaveConfig> levelWaves;
        [SerializeField] private bool loopWaves = false;

        private int currentWaveIndex = 0;

        private void Start()
        {
            StartCoroutine(SpawnAllWaves());
        }

        private IEnumerator SpawnAllWaves()
        {
            do
            {
                for (int i = currentWaveIndex; i < levelWaves.Count; i++)
                {
                    yield return StartCoroutine(SpawnWave(levelWaves[i]));
                    yield return new WaitForSeconds(levelWaves[i].waveDelay);
                }
            } while (loopWaves);
        }

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

            if (pathWave.enemyPath.pathPoints == null || pathWave.enemyPath.pathPoints.Count == 0)
            {
                Debug.LogError("Path points are not assigned correctly in EnemyPath.");
                yield break;
            }

            for (int j = 0; j < pathWave.enemyTypes.Count; j++)
            {
                GameObject enemyType = pathWave.enemyTypes[j];

                if (enemyType == null)
                {
                    Debug.LogError("Enemy type is missing in PathWave.");
                    continue;
                }

                if (j >= pathWave.enemyCounts.Count)
                {
                    Debug.LogError("Mismatch between enemyTypes and enemyCounts in PathWave.");
                    yield break;
                }

                int count = pathWave.enemyCounts[j];

                for (int i = 0; i < count; i++)
                {
                    if (pathWave.enemyPath.pathPoints[0] != null)
                    {
                        Vector3 spawnPosition = pathWave.enemyPath.pathPoints[0].position;

                        // Apply upward offset to account for enemy pivot at center
                        float yOffset = 1f; // Adjust based on the approximate height of the enemy
                        spawnPosition.y += yOffset;

                        // Raycast downward to confirm ground position
                        RaycastHit hit;
                        if (Physics.Raycast(spawnPosition + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
                        {
                            spawnPosition = hit.point + Vector3.up * yOffset; // Set to hit point + offset
                        }
                        else
                        {
                            Debug.LogWarning("No ground detected below the spawn point.");
                        }

                        GameObject newEnemy = Instantiate(enemyType, spawnPosition, Quaternion.identity);
                        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

                        if (enemyScript != null)
                        {
                            enemyScript.Initialize(100f, pathWave.enemyPath.transform);
                        }
                        else
                        {
                            Debug.LogError("Enemy prefab is missing the Enemy script component.");
                            Destroy(newEnemy);
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