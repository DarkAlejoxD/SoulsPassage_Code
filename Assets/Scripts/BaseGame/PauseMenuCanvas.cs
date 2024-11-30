using UnityEngine;
using UnityEngine.SceneManagement;
using UtilsComplements;

namespace BaseGame
{
    public class PauseMenuCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject _pause;
        [SerializeField] private GameObject _options;

        private void OnEnable()
        {
            _options.SetActive(false);
            _pause.SetActive(true);
        }

        public void Resume()
        {
            Singleton.GetSingleton<PauseManager>()?.OnResume();
        }

        public void Exit()
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}