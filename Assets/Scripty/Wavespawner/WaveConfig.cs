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
            public EnemyPath enemyPath;        // Path enemies will follow
            public List<GameObject> enemyTypes; // List of enemy types to spawn
            public List<int> enemyCounts;       // List of the number of each enemy type
            public float spawnInterval = 1f;    // Time between each enemy spawn
        }

        public List<PathWave> pathWaves = new List<PathWave>(); // Waves for each path
        public float waveDelay = 5f;   // Delay between waves for this level
    }
}