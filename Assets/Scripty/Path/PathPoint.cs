using UnityEngine;

namespace KemadaTD
{
    public class PathPoint : MonoBehaviour
    {
        public Color gizmoColor = Color.yellow;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.3f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "Path Point");
#endif
        }
    }
}
