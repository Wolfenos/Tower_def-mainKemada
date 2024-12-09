using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class EnemyPath : MonoBehaviour
    {
        public List<Transform> pathPoints = new List<Transform>();
        public List<Transform> spawnPoints = new List<Transform>();

        public List<Vector3> GetPathPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (Transform point in pathPoints)
            {
                if (point != null)
                {
                    positions.Add(point.position);
                }
            }
            return positions;
        }

        public Transform GetRandomSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Count == 0)
            {
                Debug.LogWarning("No spawn points available.");
                return null;
            }

            int randomIndex = Random.Range(0, spawnPoints.Count);
            return spawnPoints[randomIndex];
        }

        private void OnDrawGizmos()
        {
            // Draw path lines
            Gizmos.color = Color.green;
            if (pathPoints != null && pathPoints.Count > 1)
            {
                for (int i = 0; i < pathPoints.Count - 1; i++)
                {
                    if (pathPoints[i] != null && pathPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                    }
                }
            }

            // Optionally, draw lines or icons for spawn points
            Gizmos.color = Color.blue;
            if (spawnPoints != null)
            {
                foreach (Transform spawnPoint in spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    }
                }
            }
        }
    }
}
