using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UiFadeController : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;

        private void Start()
        {
            _fadeImage.color = new Color(0, 0, 0, 1);
            FadeToClear();
        }

        public void FadeToBlackAndRestart()
        {
            _fadeImage.raycastTarget = true;
            _fadeImage.DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        }

        private void FadeToClear() => _fadeImage.DOFade(0, 0.5f).OnComplete(() => _fadeImage.raycastTarget = false);
    }
}