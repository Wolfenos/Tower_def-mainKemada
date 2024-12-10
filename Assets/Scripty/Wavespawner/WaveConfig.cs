using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    [CreateAssetMenu(menuName = "KemadaTD/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        [System.Serializable]
        public class PathWave
        {
            public EnemyPath enemyPath;         // Path enemies will follow
            public List<GameObject> enemyTypes; // List of enemy types to spawn
            public List<int> enemyCounts;        // List of the number of each enemy type
            public float spawnInterval = 1f;     // Time between each enemy spawn

            // New offset fields
            [Header("Spawn Offset Settings")]
            public Vector2 xSpawnOffsetRange = new Vector2(-1f, 1f);
            public Vector2 zSpawnOffsetRange = new Vector2(-1f, 1f);

            // If you prefer a random circle:
            // public float spawnOffsetRadius = 1f;
        }

        public List<PathWave> pathWaves = new List<PathWave>(); // Waves for each path
        public float waveDelay = 5f;   // Delay between waves for this level
    }
}
