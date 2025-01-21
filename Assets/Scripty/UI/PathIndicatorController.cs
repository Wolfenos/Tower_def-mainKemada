using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PathIndicatorController : MonoBehaviour
{
    [System.Serializable]
    public class PathIndicator
    {
        [Header("Basic Settings")]
        [Tooltip("Friendly name to identify the path in the inspector (optional).")]
        public string pathName;

        [Tooltip("Where enemies spawn or entrance location in the world.")]
        public Transform pathEntrance;

        [Header("World-Space Arrow (3D)")]
        [Tooltip("Optional: If you want a 3D arrow in the scene, assign its prefab or reference.")]
        public GameObject worldSpaceArrowPrefab;

        [Tooltip("Should we automatically instantiate this arrow? Or is it already in the scene?")]
        public bool instantiateWorldSpaceArrow = true;

        [Tooltip("Offset from path entrance, if you want the arrow to hover above or near the path.")]
        public Vector3 arrowPositionOffset = new Vector3(0, 2, 0);

        [Header("Screen-Space Arrow (UI)")]
        [Tooltip("Optional: If you want a UI arrow that appears on-screen (e.g., pinned to the edge or floating).")]
        public GameObject screenSpaceArrowPrefab;

        [Tooltip("Where to place the UI arrow? Could be a child RectTransform in your Canvas.")]
        public RectTransform screenSpaceArrowParent;

        [Header("Highlight / Animation Settings")]
        [Tooltip("Color to flash or highlight the arrow when active.")]
        public Color highlightColor = Color.red;

        [Tooltip("Speed of pulsing or rotating animation.")]
        public float pulseSpeed = 2f;

        [Tooltip("Should the arrow pulse in/out, rotate, or remain static?")]
        public bool shouldPulse = true;

        [Tooltip("Should the arrow spin/rotate around its Y-axis? (For world-space arrow)")]
        public bool shouldRotate = true;

        // Internal references (instantiated arrow objects)
        [HideInInspector] public GameObject worldSpaceArrowInstance;
        [HideInInspector] public GameObject screenSpaceArrowInstance;
        [HideInInspector] public bool isActive;
    }

    [Header("Path Indicators")]
    [Tooltip("Add one element per path.")]
    public PathIndicator[] pathIndicators;

    [Header("General Animation Controls")]
    [Tooltip("If true, will pulse alpha/color of active arrow(s). If false, color remains constant.")]
    public bool globalPulseEnabled = true;

    [Tooltip("If true, will rotate active arrow(s). If false, arrow(s) remain static.")]
    public bool globalRotateEnabled = true;

    void Start()
    {
        // Instantiate arrow prefabs if requested, and hide them initially
        for (int i = 0; i < pathIndicators.Length; i++)
        {
            PathIndicator p = pathIndicators[i];

            // World-space arrow
            if (p.worldSpaceArrowPrefab && p.instantiateWorldSpaceArrow)
            {
                p.worldSpaceArrowInstance = Instantiate(
                    p.worldSpaceArrowPrefab,
                    p.pathEntrance.position + p.arrowPositionOffset,
                    Quaternion.identity
                );
                p.worldSpaceArrowInstance.SetActive(false);
            }
            else
            {
                // If it's already in the scene or if not used
                p.worldSpaceArrowInstance = p.worldSpaceArrowPrefab;
                if (p.worldSpaceArrowInstance != null)
                    p.worldSpaceArrowInstance.SetActive(false);
            }

            // Screen-space arrow
            if (p.screenSpaceArrowPrefab && p.screenSpaceArrowParent)
            {
                p.screenSpaceArrowInstance = Instantiate(
                    p.screenSpaceArrowPrefab,
                    p.screenSpaceArrowParent
                );
                p.screenSpaceArrowInstance.SetActive(false);
            }
            else
            {
                p.screenSpaceArrowInstance = p.screenSpaceArrowPrefab;
                if (p.screenSpaceArrowInstance != null)
                    p.screenSpaceArrowInstance.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Handle pulsing and/or rotation for active arrows
        for (int i = 0; i < pathIndicators.Length; i++)
        {
            PathIndicator p = pathIndicators[i];
            if (!p.isActive) continue;

            // World-space arrow
            if (p.worldSpaceArrowInstance != null && p.worldSpaceArrowInstance.activeSelf)
            {
                // Optional rotation
                if (globalRotateEnabled && p.shouldRotate)
                {
                    p.worldSpaceArrowInstance.transform.Rotate(Vector3.up * (45f * Time.deltaTime));
                }

                // Optional pulsing (color or scale)
                if (globalPulseEnabled && p.shouldPulse)
                {
                    // Example: pulsing scale
                    float scale = 1 + 0.1f * Mathf.Sin(Time.time * p.pulseSpeed);
                    p.worldSpaceArrowInstance.transform.localScale = new Vector3(scale, scale, scale);

                    // If you have a renderer, you could also pulse color:
                    // var renderer = p.worldSpaceArrowInstance.GetComponentInChildren<Renderer>();
                    // if (renderer != null)
                    // {
                    //     float t = (Mathf.Sin(Time.time * p.pulseSpeed) + 1f) * 0.5f; // 0..1
                    //     Color c = Color.Lerp(Color.white, p.highlightColor, t);
                    //     renderer.material.color = c;
                    // }
                }
            }

            // Screen-space arrow
            if (p.screenSpaceArrowInstance != null && p.screenSpaceArrowInstance.activeSelf)
            {
                // If it's a UI element, you might rotate it or animate it similarly:
                if (globalRotateEnabled && p.shouldRotate)
                {
                    p.screenSpaceArrowInstance.transform.Rotate(Vector3.forward * (45f * Time.deltaTime));
                }

                if (globalPulseEnabled && p.shouldPulse)
                {
                    // Example with pulsing alpha if it has an Image
                    Image img = p.screenSpaceArrowInstance.GetComponent<Image>();
                    if (img)
                    {
                        float t = (Mathf.Sin(Time.time * p.pulseSpeed) + 1f) * 0.5f; // 0..1
                        Color c = p.highlightColor;
                        c.a = Mathf.Lerp(0.5f, 1f, t);
                        img.color = c;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Call this to show the indicator for a specific path.
    /// Example: ShowIndicator(0) to show the first path's arrow(s).
    /// </summary>
    public void ShowIndicator(int pathIndex)
    {
        if (pathIndex < 0 || pathIndex >= pathIndicators.Length) return;

        PathIndicator p = pathIndicators[pathIndex];
        p.isActive = true;

        // Show World-space arrow
        if (p.worldSpaceArrowInstance != null)
        {
            p.worldSpaceArrowInstance.SetActive(true);
            // Optionally position it again if needed
            p.worldSpaceArrowInstance.transform.position = p.pathEntrance.position + p.arrowPositionOffset;
        }

        // Show Screen-space arrow
        if (p.screenSpaceArrowInstance != null)
        {
            p.screenSpaceArrowInstance.SetActive(true);
            // Optionally position it. 
            // For a simple approach, you might place it at your pathEntrance in world coords => UI coords
            // or just keep it in a certain corner. 
            // Example (center it in the parent):
            p.screenSpaceArrowInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// Call this to hide the indicator for a specific path.
    /// Example: HideIndicator(0) to hide the first path's arrow(s).
    /// </summary>
    public void HideIndicator(int pathIndex)
    {
        if (pathIndex < 0 || pathIndex >= pathIndicators.Length) return;

        PathIndicator p = pathIndicators[pathIndex];
        p.isActive = false;

        if (p.worldSpaceArrowInstance != null)
            p.worldSpaceArrowInstance.SetActive(false);

        if (p.screenSpaceArrowInstance != null)
            p.screenSpaceArrowInstance.SetActive(false);
    }

    /// <summary>
    /// Convenience method to toggle a path’s indicator on/off.
    /// </summary>
    public void ToggleIndicator(int pathIndex, bool show)
    {
        if (show) ShowIndicator(pathIndex);
        else HideIndicator(pathIndex);
    }
}
