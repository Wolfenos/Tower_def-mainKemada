// AnchorHighlighter.cs
using UnityEngine;

public class AnchorHighlighter : MonoBehaviour
{
    private Renderer anchorRenderer;
    private Color originalColor;
    [SerializeField] private Color highlightColor = Color.yellow; // Set highlight color in Inspector

    private void Start()
    {
        anchorRenderer = GetComponent<Renderer>();
        originalColor = anchorRenderer.material.color;
    }

    public void Highlight(bool shouldHighlight)
    {
        anchorRenderer.material.color = shouldHighlight ? highlightColor : originalColor;
    }
}
