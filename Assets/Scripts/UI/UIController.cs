using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [Header("UI Screens")]
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private GameObject _gameOverScreen;
        [SerializeField] private GameObject _victoryScreen;

        [Header("References")]
        [SerializeField] private CarController _carController;
        [SerializeField] private UiFadeController _fadeController;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _playAgainButton;
    
        [Header("Animations")]
        [SerializeField] private Animator _cameraAnimator;
    
        private void Start()
        {
            _carController.ON_PLAYER_WIN += ShowVictoryScreen;
            _carController.ON_PLAYER_LOSE += ShowGameOverScreen;
        
            _playButton.onClick.AddListener(OnPlayButton);
            _restartButton.onClick.AddListener(PlayAgain);
            _playAgainButton.onClick.AddListener(PlayAgain);
        
            ShowStartScreen();
        }

        private void ShowStartScreen()
        {
            _startScreen.SetActive(true);
            _gameOverScreen.SetActive(false);
            _victoryScreen.SetActive(false);
        }

        private void ShowGameOverScreen() => _gameOverScreen.SetActive(true);

        private void ShowVictoryScreen() => _victoryScreen.SetActive(true);

        private void OnPlayButton()
        {
            _cameraAnimator.SetTrigger("ToGameplayCamera");
            _startScreen.SetActive(false);
            _carController.StartMoving();
        }

        private void PlayAgain() => _fadeController.FadeToBlackAndRestart();

        private void OnDestroy()
        {
            _carController.ON_PLAYER_WIN -= ShowVictoryScreen;
            _carController.ON_PLAYER_LOSE -= ShowGameOverScreen;
        
            _playButton.onClick.RemoveListener(OnPlayButton);
            _restartButton.onClick.RemoveListener(PlayAgain);
            _playAgainButton.onClick.RemoveListener(PlayAgain);
        }
    }
}