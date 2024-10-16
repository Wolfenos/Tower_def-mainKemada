using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class WaveManager : MonoBehaviour
    {
        public List<WaveConfig> waveConfigs; // List of wave configurations
        public PathManager pathManager;      // Reference to the PathManager

        private void Start()
        {
            StartCoroutine(SpawnAllWaves());
        }

        private IEnumerator SpawnAllWaves()
        {
            foreach (WaveConfig wave in waveConfigs)
            {
                yield return StartCoroutine(SpawnEnemiesInWave(wave));
            }
        }

        private IEnumerator SpawnEnemiesInWave(WaveConfig waveConfig)
        {
            // Check if pathManager is assigned
            if (pathManager == null)
            {
                Debug.LogError("PathManager is not assigned in WaveManager.");
                yield break;
            }

            // Retrieve the path based on the index from WaveConfig
            EnemyPath selectedPath = pathManager.GetPath(waveConfig.pathIndex);

            // Check if the selected path is valid
            if (selectedPath == null)
            {
                Debug.LogError("Selected path is null. Ensure that the path index is correct in the WaveConfig.");
                yield break;
            }

            for (int i = 0; i < waveConfig.numberOfEnemies; i++)
            {
                // Check if the path has points
                if (selectedPath.pathPoints == null || selectedPath.pathPoints.Count == 0 || selectedPath.pathPoints[0] == null)
                {
                    Debug.LogError("Path points are not assigned correctly in EnemyPath.");
                    yield break;
                }

                // Instantiate enemy at the first path point position
                GameObject newEnemy = Instantiate(waveConfig.enemyPrefab, selectedPath.pathPoints[0].transform.position, Quaternion.identity);

                // Check if the enemy has the Enemy script
                Enemy enemyScript = newEnemy.GetComponent<Enemy>();
                if (enemyScript == null)
                {
                    Debug.LogError("Enemy prefab is missing the Enemy script component.");
                    Destroy(newEnemy);
                    yield break;
                }

                // Initialize the enemy with the path
                enemyScript.Initialize(selectedPath);

                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
    }
}