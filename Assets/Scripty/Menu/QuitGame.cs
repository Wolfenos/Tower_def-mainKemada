using UnityEngine;

namespace KemadaTD
{
    public class QuitGame : MonoBehaviour
    {
        // Method to quit the game
        public void Quit()
        {
#if UNITY_EDITOR
            Debug.Log("Game exited in editor mode");
            UnityEditor.EditorApplication.isPlaying = false; // Only for testing in the editor
#else
            Application.Quit(); // Quits the game in a built application
#endif
        }
    }
}
