using UnityEngine;

namespace KemadaTD
{
    public class GhostTurret : MonoBehaviour
    {
        [Header("Materials")]
        public Material validMaterial;    // Material to indicate a valid build position
        public Material invalidMaterial;  // Material to indicate an invalid build position

        private Renderer ghostRenderer;

        private void Awake()
        {
            ghostRenderer = GetComponentInChildren<Renderer>();
            SetTransparent(true);
        }

        // Set the ghost turret position and validate placement
        public void UpdatePosition(Vector3 position, bool isValid)
        {
            transform.position = position;
            ghostRenderer.material = isValid ? validMaterial : invalidMaterial;
        }

        // Make the turret semi-transparent
        private void SetTransparent(bool transparent)
        {
            Color color = ghostRenderer.material.color;
            color.a = transparent ? 0.5f : 1f;
            ghostRenderer.material.color = color;
        }
    }
}
