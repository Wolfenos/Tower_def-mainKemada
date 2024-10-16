using System.Collections.Generic;
using UnityEngine;

namespace KemadaTD
{
    public class PathManager : MonoBehaviour
    {
        public List<EnemyPath> paths = new List<EnemyPath>(); // List of paths to manage

        private void OnDrawGizmos()
        {
            // Draw all paths with their assigned colors
            foreach (EnemyPath path in paths)
            {
                if (path != null)
                {
                    path.OnDrawGizmos();
                }
            }
        }

        // Method to get a specific path
        public EnemyPath GetPath(int index)
        {
            if (index >= 0 && index < paths.Count)
                return paths[index];

            return null;
        }
    }
}