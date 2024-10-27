using UnityEngine;
using UnityEngine.SceneManagement;

namespace KemadaTD
{
    public class SceneChanger : MonoBehaviour
    {
        [Tooltip("Specify the name of the scene to load.")]
        [SerializeField] private string sceneName;

        // Method to change to the specified scene
        public void ChangeScene()
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("Scene name is not specified!");
            }
        }
    }
}
