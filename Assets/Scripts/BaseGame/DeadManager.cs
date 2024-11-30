using AudioController;
using System.Collections;
using UnityEngine.UI;

using UnityEngine;
using UtilsComplements;

namespace BaseGame
{
    public class DeadManager : MonoBehaviour, ISingleton<DeadManager>
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Image _image;

        [Header("Attributes")]
        [SerializeField, Min(0.1f)] private float _appearTime;
        [SerializeField] private AnimationCurve _appearCurve;
        [SerializeField, Min(0.1f)] private float _stayTime;
        [SerializeField, Min(0.1f)] private float _disappearTime;
        [SerializeField] private AnimationCurve _disappearCurve;
        #endregion

        public ISingleton<DeadManager> Instance => this;

        #region Unity Logic
        private void Awake() => Instance.Instantiate();
        private void OnDestroy() => Instance.RemoveInstance();
        private void Start() => _image.enabled = false;
        #endregion

        #region Static Methods
        public static void ActivateDead()
        {
            if (!Singleton.TryGetInstance(out DeadManager manager))
            {
                GameManager.ResetGame();
                return;
            }

            manager.StartCoroutine(manager.DeadCoroutine());
        }
        #endregion

        #region Private Methods
        private IEnumerator DeadCoroutine()
        {
            AudioManager.GetAudioManager().PlayOneShot(Database.Player, "DEAD", transform.position);
            Singleton.GetSingleton<FakeTransparenceControl>()?.SphereMeshSetActive(false);
            Color color = _image.color;
            color.a = 0;
            _image.color = color;
            _image.enabled = true;

            GameManager.Player?.BlockMovement();

            #region AppearLogic
            for (float i = 0; i <= _appearTime; i += Time.deltaTime)
            {
                color.a = _appearCurve.Evaluate(i / _appearTime);
                _image.color = color;
                //if (false)
                //{
                //    yield return new WaitWhile(() => false);
                //}
                yield return new WaitForSeconds(Time.deltaTime);
            }
            color.a = 1;
            _image.color = color;
            #endregion

            #region Make ReappearLogic
            float halfStay = _stayTime / 2;

            //AudioManager.GetAudioManager().PlayOneShot();
            yield return new WaitForSeconds(halfStay);

            GameManager.ResetGame();

            yield return new WaitForSeconds(halfStay);
            #endregion

            #region Dissapear
            for (float i = 0; i <= _disappearTime; i += Time.deltaTime)
            {
                color.a = _disappearCurve.Evaluate(i / _disappearTime);
                _image.color = color;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            color.a = 0;
            _image.color = color;
            _image.enabled = false;
            #endregion

            GameManager.Player?.UnBlockMovement();
            Singleton.GetSingleton<FakeTransparenceControl>()?.SphereMeshSetActive(true);
        }
        #endregion
    }
}
