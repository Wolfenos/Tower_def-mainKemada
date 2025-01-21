using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor; // Needed for Handles in OnDrawGizmos
#endif

namespace KemadaTD
{
    public class CircleController : MonoBehaviour
    {
        // Custom key bindings for controlling rings (set in Inspector)
        [System.Serializable]
        public class CircleInput
        {
            public KeyCode selectNextRingKey = KeyCode.Tab;     // Key to select the next ring
            public KeyCode rotateLeftKey = KeyCode.LeftArrow;   // Key to rotate left
            public KeyCode rotateRightKey = KeyCode.RightArrow; // Key to rotate right
        }

        [System.Serializable]
        public class Sector
        {
            public string name;                           // Name of the sector (for identification)
            [Range(0f, 360f)] public float startAngle;    // Start angle in degrees
            [Range(0f, 360f)] public float endAngle;      // End angle in degrees
            public bool isActive;                         // Active checkbox
            public bool isPassive;                        // Passive checkbox
            public Color color = Color.green;             // Color for visualization
        }

        public CircleInput inputSettings;                 // Input settings for selecting and rotating rings
        public Transform[] rings;                        // Array of rings to control
        public List<Sector> sectors = new List<Sector>(); // List of sectors

        [Header("Rotation Settings")]
        public float rotationAmount = 90f;               // How much to rotate per input (e.g., 90 degrees)
        public float rotationDuration = 0.5f;            // Duration for the rotation animation
        public float rotationSpeed = 1.0f;               // Speed of the rotation animation, editable in the inspector

        // ---------------- Highlight Settings (GLOBAL) ----------------
        [Header("Highlight Settings (Global Defaults)")]
        [Range(0.01f, 0.5f)]
        [Tooltip("Thickness of the highlight line.")]
        public float highlightWidth = 0.1f;

        [Range(3, 64)]
        [Tooltip("Number of segments used to draw the circular highlight.")]
        public int highlightSegments = 36;

        [Tooltip("How long (in seconds) the highlight remains fully visible after selection/rotation. 0 = never hide.")]
        public float highlightDisappearTime = 0f;

        [Tooltip("How long (in seconds) the highlight takes to fade out once the disappear time is reached.")]
        public float highlightFadeDuration = 2f;

        // ---------------- Per-Ring Customization ----------------
        [Tooltip("Colors for each ring's highlight. If empty or shorter than ring count, a default color is used.")]
        public Color[] ringHighlightColors;

        [Tooltip("Per-ring vertical offsets for the highlight. Match array length to the number of rings.")]
        public float[] ringHighlightYOffsets;

        [Tooltip("Per-ring radii for the highlight circle. If empty or shorter than ring count, uses a default (e.g. 3f).")]
        public float[] ringHighlightRadii;

        private int currentRingIndex = 0;    // Index of the currently selected ring
        private bool isRotating = false;     // To check if a rotation is in progress

        // We'll store a LineRenderer for each ring to show its highlight circle
        private LineRenderer[] ringHighlights;

        // Track highlight start time for each ring (for fade logic)
        private float[] ringHighlightStartTimes;
        // Track if a ring is currently fading out
        private bool[] ringFadingOut;
        // Keep reference to any running fade-out coroutines
        private Coroutine[] ringFadeCoroutines;

        private void Start()
        {
            // Create a highlight circle (LineRenderer) for each ring
            CreateHighlightCircles();

            // Initialize arrays for fade logic
            ringHighlightStartTimes = new float[rings.Length];
            ringFadingOut = new bool[rings.Length];
            ringFadeCoroutines = new Coroutine[rings.Length];

            // Show the highlight for the initial ring
            ringHighlightStartTimes[currentRingIndex] = Time.time;
            ShowHighlight(currentRingIndex, true); // Force show
        }

        private void Update()
        {
            // Change selection of the current ring
            if (Input.GetKeyDown(inputSettings.selectNextRingKey) && !isRotating)
            {
                // Hide old ring (stop fade if it’s fading)
                HideHighlight(currentRingIndex);

                // Select next ring
                currentRingIndex = (currentRingIndex + 1) % rings.Length;

                // Show new ring
                ShowHighlight(currentRingIndex, true);
            }

            // Rotate the currently selected ring based on input
            if (Input.GetKeyDown(inputSettings.rotateLeftKey) && !isRotating)
            {
                StartCoroutine(RotateRing(rings[currentRingIndex], -rotationAmount));
            }
            else if (Input.GetKeyDown(inputSettings.rotateRightKey) && !isRotating)
            {
                StartCoroutine(RotateRing(rings[currentRingIndex], rotationAmount));
            }

            // Check if highlight should start to fade (for the currently selected ring)
            if (highlightDisappearTime > 0f && !ringFadingOut[currentRingIndex])
            {
                float elapsed = Time.time - ringHighlightStartTimes[currentRingIndex];
                if (elapsed >= highlightDisappearTime)
                {
                    // Time is up, start fade out
                    StartFadeOut(currentRingIndex);
                }
            }
        }

        // Rotate the ring by a given number of degrees with animation
        private IEnumerator RotateRing(Transform ring, float degrees)
        {
            isRotating = true;
            Quaternion startRotation = ring.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(0, degrees, 0);
            float elapsedTime = 0f;

            // Reset the highlight timer so it won't fade while rotating
            ShowHighlight(currentRingIndex, true);

            while (elapsedTime < rotationDuration)
            {
                ring.rotation = Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    (elapsedTime / rotationDuration) * rotationSpeed
                );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ring.rotation = endRotation;
            isRotating = false;
        }

        /// <summary>
        /// Create the highlight circles with a URP Unlit Transparent material.
        /// </summary>
        private void CreateHighlightCircles()
        {
            ringHighlights = new LineRenderer[rings.Length];

            for (int i = 0; i < rings.Length; i++)
            {
                // Create an empty child object on each ring for the highlight
                GameObject highlightObject = new GameObject("RingHighlight_" + i);
                highlightObject.transform.SetParent(rings[i], false);

                // Set local position to handle Y offset
                float yOffset = 0f;
                if (ringHighlightYOffsets != null && ringHighlightYOffsets.Length > i)
                {
                    yOffset = ringHighlightYOffsets[i];
                }
                highlightObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
                highlightObject.transform.localRotation = Quaternion.identity;

                // Add a LineRenderer
                LineRenderer lr = highlightObject.AddComponent<LineRenderer>();

                // 1) Find URP Unlit shader
                Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
                if (!urpUnlit)
                {
                    Debug.LogWarning("Could not find 'Universal Render Pipeline/Unlit' shader. Using 'Sprites/Default' as fallback.");
                    urpUnlit = Shader.Find("Sprites/Default");
                }

                // 2) Create a material from this shader
                Material mat = new Material(urpUnlit);

                // 3) Configure the material for transparency
                if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f); // 0 = Opaque, 1 = Transparent
                if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 0f);   // 0 = Alpha blend
                if (mat.HasProperty("_ZWrite")) mat.SetFloat("_ZWrite", 0f);  // Turn off ZWrite for transparency
                // Force the render queue to Transparent
                mat.renderQueue = 3000;

                // Enable alpha blend keywords if needed
                mat.EnableKeyword("_ALPHATEST_OFF");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_OFF");

                // Assign the material to our LineRenderer
                lr.material = mat;

                // 4) Configure the LineRenderer
                lr.useWorldSpace = false;  // We'll define positions in local space
                lr.loop = true;            // Makes the line continuous (looped circle)
                lr.numCapVertices = 2;     // Smooth circle edges (optional)
                lr.startWidth = highlightWidth;
                lr.endWidth = highlightWidth;

                // Determine the radius for this ring
                float ringRadius = 3f; // fallback
                if (ringHighlightRadii != null && ringHighlightRadii.Length > i)
                {
                    ringRadius = ringHighlightRadii[i];
                }

                // Generate the circle points
                lr.positionCount = highlightSegments;
                float angleStep = 360f / highlightSegments;
                for (int j = 0; j < highlightSegments; j++)
                {
                    float angle = Mathf.Deg2Rad * (j * angleStep);
                    float x = Mathf.Cos(angle) * ringRadius;
                    float z = Mathf.Sin(angle) * ringRadius;
                    lr.SetPosition(j, new Vector3(x, 0f, z));
                }

                // Hide the highlight initially
                highlightObject.SetActive(false);
                ringHighlights[i] = lr;
            }
        }

        // ----------------------------------------------------------------
        // Show / Hide / Fade Out logic
        // ----------------------------------------------------------------

        /// <summary>
        /// Immediately shows (and resets alpha to fully opaque) the highlight for a ring.
        /// Also resets that ring's fade timer.
        /// </summary>
        private void ShowHighlight(int ringIndex, bool resetTimer)
        {
            if (ringHighlights[ringIndex] == null) return;

            // If a fade-out was in progress, stop it
            if (ringFadeCoroutines[ringIndex] != null)
            {
                StopCoroutine(ringFadeCoroutines[ringIndex]);
                ringFadeCoroutines[ringIndex] = null;
            }

            ringFadingOut[ringIndex] = false;

            // Make sure the ring highlight is active
            ringHighlights[ringIndex].gameObject.SetActive(true);

            // Reset alpha to 1.0 on _BaseColor
            Material mat = ringHighlights[ringIndex].material;
            if (mat.HasProperty("_BaseColor"))
            {
                // If we have a ring color set, use that. Otherwise, default to yellow.
                Color ringColor = (ringHighlightColors != null && ringHighlightColors.Length > ringIndex)
                                  ? ringHighlightColors[ringIndex]
                                  : Color.yellow;

                ringColor.a = 1f; // fully opaque
                mat.SetColor("_BaseColor", ringColor);
            }

            // Update line width in case it changed
            ringHighlights[ringIndex].startWidth = highlightWidth;
            ringHighlights[ringIndex].endWidth = highlightWidth;

            // Reset the highlight start time if desired
            if (resetTimer)
            {
                ringHighlightStartTimes[ringIndex] = Time.time;
            }
        }

        /// <summary>
        /// Immediately hides the highlight (no fade).
        /// E.g., if you're switching to another ring, you can hide the old ring instantly.
        /// </summary>
        private void HideHighlight(int ringIndex)
        {
            if (ringHighlights[ringIndex] == null) return;

            // Stop any fade out in progress
            if (ringFadeCoroutines[ringIndex] != null)
            {
                StopCoroutine(ringFadeCoroutines[ringIndex]);
                ringFadeCoroutines[ringIndex] = null;
            }

            ringFadingOut[ringIndex] = false;
            ringHighlights[ringIndex].gameObject.SetActive(false);
        }

        /// <summary>
        /// Start fading out the highlight for the specified ring (over highlightFadeDuration).
        /// </summary>
        private void StartFadeOut(int ringIndex)
        {
            if (ringHighlights[ringIndex] == null) return;
            if (ringFadingOut[ringIndex]) return; // Already fading

            ringFadingOut[ringIndex] = true;
            ringFadeCoroutines[ringIndex] = StartCoroutine(FadeOutHighlight(ringIndex));
        }

        /// <summary>
        /// Coroutine that gradually reduces the highlight alpha from 1 to 0 over highlightFadeDuration,
        /// then deactivates the highlight.
        /// </summary>
        private IEnumerator FadeOutHighlight(int ringIndex)
        {
            LineRenderer lr = ringHighlights[ringIndex];
            if (lr == null) yield break;

            Material mat = lr.material;
            if (mat == null) yield break;

            float elapsed = 0f;
            // We'll read the initial alpha in case it isn't exactly 1
            float startAlpha = 1f;

            // If the material has _BaseColor, read that alpha
            if (mat.HasProperty("_BaseColor"))
            {
                Color initialColor = mat.GetColor("_BaseColor");
                startAlpha = initialColor.a;
            }

            while (elapsed < highlightFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / highlightFadeDuration);

                float newAlpha = Mathf.Lerp(startAlpha, 0f, t);

                // Update _BaseColor alpha if possible
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c = mat.GetColor("_BaseColor");
                    c.a = newAlpha;
                    mat.SetColor("_BaseColor", c);
                }

                yield return null;
            }

            // Ensure alpha is fully 0
            if (mat.HasProperty("_BaseColor"))
            {
                Color finalColor = mat.GetColor("_BaseColor");
                finalColor.a = 0f;
                mat.SetColor("_BaseColor", finalColor);
            }

            // Deactivate highlight
            lr.gameObject.SetActive(false);

            ringFadingOut[ringIndex] = false;
            ringFadeCoroutines[ringIndex] = null;
        }

        // ----------------------------------------------------------------
        // Editor-only sector drawing (unchanged)
        // ----------------------------------------------------------------
        private void OnDrawGizmos()
        {
            DrawSectors();
        }

        private void DrawSectors()
        {
            if (sectors == null || sectors.Count == 0) return;

            float radius = 10f; // Adjust as needed for visualization
            Vector3 center = transform.position; // Center point for sectors

#if UNITY_EDITOR
            // Save the current color
            Color oldColor = Handles.color;

            foreach (Sector sector in sectors)
            {
                // Set the color based on the sector's color
                Handles.color = sector.color;

                // Calculate the angles for the sector
                float startAngle = sector.startAngle;
                float endAngle = sector.endAngle;

                // Ensure startAngle < endAngle
                if (endAngle < startAngle)
                {
                    endAngle += 360f;
                }

                // Draw the sector as a filled arc in world space
                Handles.DrawSolidArc(
                    center,
                    Vector3.up,
                    Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward,
                    endAngle - startAngle,
                    radius
                );

                // Optionally, draw sector boundaries
                Handles.color = Color.black;
                Vector3 from = Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward * radius;
                Vector3 to = Quaternion.Euler(0f, endAngle, 0f) * Vector3.forward * radius;
                Handles.DrawLine(center, center + from);
                Handles.DrawLine(center, center + to);
            }

            // Restore old color
            Handles.color = oldColor;
#endif
        }
    }
}
