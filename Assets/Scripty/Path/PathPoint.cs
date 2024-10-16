using UnityEngine;

namespace KemadaTD
{
    public class PathPoint : MonoBehaviour
    {
        public Color gizmoColor = Color.yellow; // Color for visualizing this point in the editor

        private void OnDrawGizmos()
        {
            // Draw a sphere to represent this path point
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
    }
}