using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class EnemyPath : MonoBehaviour
    {
        public List<PathPoint> pathPoints = new List<PathPoint>(); // Points along this path
        public Color pathColor = Color.green; // Color for visualizing the path

        // Change the access modifier of OnDrawGizmos to public
        public void OnDrawGizmos()
        {
            if (pathPoints == null || pathPoints.Count < 2)
                return;

            Gizmos.color = pathColor;

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(pathPoints[i].transform.position, pathPoints[i + 1].transform.position);
                }
            }
        }

        // Method to get the positions of each path point
        public List<Vector3> GetPathPositions()
        {
            List<Vector3> positions = new List<Vector3>();

            foreach (PathPoint point in pathPoints)
            {
                if (point != null)
                {
                    positions.Add(point.transform.position);
                }
            }

            return positions;
        }
    }
}