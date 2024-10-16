using UnityEngine;

namespace KemadaTD
{
    [CreateAssetMenu(menuName = "KemadaTD/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public GameObject enemyPrefab; // Prefab for the enemy
        public int pathIndex = 0;      // Index of the path in PathManager
        public int numberOfEnemies = 5;  // Number of enemies in this wave
        public float spawnInterval = 1f; // Time between spawns
    }
}