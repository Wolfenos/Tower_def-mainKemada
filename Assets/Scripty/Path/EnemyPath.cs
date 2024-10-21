using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class EnemyPath : MonoBehaviour
    {
        public string pathPointTag = "PathPoint";    // Tag used to find path points
        public List<PathPoint> pathPoints = new List<PathPoint>(); // List of PathPoint objects along the path

        private void Awake()
        {
            // Find all GameObjects tagged as path points and add them to pathPoints list
            GameObject[] points = GameObject.FindGameObjectsWithTag(pathPointTag);

            foreach (GameObject point in points)
            {
                PathPoint pathPoint = point.GetComponent<PathPoint>();
                if (pathPoint != null)
                {
                    pathPoints.Add(pathPoint);
                }
            }
        }

        // Method to get the positions of all path points as a list of Vector3 positions
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

        // Change access modifier to public so it can be accessed by PathManager
        public void OnDrawGizmos()
        {
            if (pathPoints == null || pathPoints.Count < 2)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                if (pathPoints[i] != null && pathPoints[i + 1] != null)
                {
                    Gizmos.DrawLine(pathPoints[i].transform.position, pathPoints[i + 1].transform.position);
                }
            }
        }
    }
}