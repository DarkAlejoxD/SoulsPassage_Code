using UnityEngine;
using UnityEngine.SceneManagement;
using static UtilsComplements.AsyncTimer;

namespace BaseGame
{
    public class ChangeScene : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float _delayTime = 0.1f;
        [SerializeField, Range(0, 2)] private int _sceneIndex = 0;
        [SerializeField] private bool _playOnStart = false;
        [SerializeField, TextArea(3, 4)]
        private string _scenes =
            "0: Menu" +
            "\n1: Game" +
            "\n2:Credits";

        private void Start()
        {
            if (_playOnStart)
                LoadScene();
        }

        public void LoadScene()
        {
            StartCoroutine(TimerCoroutine(_delayTime, Change));
        }

        private void Change()
        {
            SceneManager.LoadSceneAsync(_sceneIndex);
        }
    }
}
