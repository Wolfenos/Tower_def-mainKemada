using UnityEngine;

namespace KemadaTD
{
    public class SpawnPoint : MonoBehaviour
    {
        public Color gizmoColor = Color.blue; // Different color to distinguish spawn points

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.4f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "Spawn Point");
#endif
        }
    }
}
