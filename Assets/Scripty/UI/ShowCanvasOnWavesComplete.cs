using UnityEngine;

public class ShowCanvasOnWavesComplete : MonoBehaviour
{
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private KemadaTD.WaveManager waveManager;

    private void Awake()
    {
        // Ensure the end-game canvas is hidden at start
        if (endGameCanvas != null)
            endGameCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted += HandleAllWavesCompleted;
        }
    }

    private void OnDisable()
    {
        if (waveManager != null)
        {
            waveManager.OnAllWavesCompleted -= HandleAllWavesCompleted;
        }
    }

    private void HandleAllWavesCompleted()
    {
        if (endGameCanvas != null)
        {
            // Show the end-game canvas
            endGameCanvas.SetActive(true);
        }
    }
}
