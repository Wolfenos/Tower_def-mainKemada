using UnityEngine;
using UnityEngine.UI;

namespace KemadaTD
{
    public class GameSpeedController : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button fastForwardButton;

        private void Start()
        {
            // Add listeners to buttons to call respective functions when clicked
            pauseButton.onClick.AddListener(PauseGame);
            playButton.onClick.AddListener(PlayGame);
            fastForwardButton.onClick.AddListener(FastForwardGame);
        }

        private void PauseGame()
        {
            Time.timeScale = 0;
            Debug.Log("Game Paused");
        }

        private void PlayGame()
        {
            Time.timeScale = 1;
            Debug.Log("Game Playing at Normal Speed");
        }

        private void FastForwardGame()
        {
            Time.timeScale = 2;
            Debug.Log("Game Fast Forwarded");
        }

        private void OnDisable()
        {
            // Reset the time scale when the object is disabled
            Time.timeScale = 1;
        }
    }
}
