using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class EnemyPath : MonoBehaviour
    {
        // List to manually assign path points in the Inspector
        public List<Transform> pathPoints = new List<Transform>();

        // Method to get the positions of all path points as a list of Vector3 positions
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

        // Draws lines between path points in the Unity Editor for easy visualization
        public void OnDrawGizmos()
        {
            if (pathPoints == null || pathPoints.Count < 2)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                }
            }
        }
    }
}