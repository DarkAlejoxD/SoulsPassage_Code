using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuManagement
{    
    public class MenuController : MonoBehaviour
    {  
        #region Public Methods        
        public void ChangeScene(int sceneIndex)
        {
            SceneManager.LoadSceneAsync(sceneIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}
