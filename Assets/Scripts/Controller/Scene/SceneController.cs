using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller.Scene
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance;
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void SwitchSceneTo(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
