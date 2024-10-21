using Game.Levels;
using UnityEngine;
using VContainer;

namespace Game.UI.Screens
{
    public class UIIngameScreen : UIScreen
    {
        [SerializeField] private GameObject _waitToStartUI;
        [SerializeField] private UIButton _startButton;
        [SerializeField] private GameObject _runtimeObject;

        private UIManager _uiManager;
        private LevelManager _levelManager;
        private LevelController _levelController;
        
        
        [Inject]
        private void Install(LevelManager levelManager, UIManager uiManager)
        {
            _levelManager = levelManager;
            _uiManager = uiManager;
        }

        private void OnEnable()
        {
            SetState(false);
            _startButton.OnClick += OnStartClick;
            
            SetLevelController(_levelManager.LevelController);
            _levelManager.OnControllerChanged += OnLevelControllerChangeHandler;
        }

        protected override void OnDisable()
        {
            _startButton.OnClick -= OnStartClick;
            _levelManager.OnControllerChanged -= OnLevelControllerChangeHandler;
            base.OnDisable();
        }

        private void OnLevelControllerChangeHandler(LevelController levelController)
        {
            SetLevelController(levelController);
        }

        private void SetLevelController(LevelController levelController)
        {
            if (_levelController != null)
            {
                _levelController.OnLevelCompleted -= OnLevelCompletedHandler;
            }

            _levelController = levelController;

            if (_levelController != null)
            {
                _levelController.OnLevelCompleted += OnLevelCompletedHandler;
            }
        }

        private void OnLevelCompletedHandler(bool isWin)
        {
            _uiManager.HideElement(this);
        }

        private void OnStartClick()
        {
            SetState(true);
            if (_levelController != null)
            {
                _levelController.StartLevel();
            }
        }

        private void SetState(bool isRuntime)
        {
            _waitToStartUI.gameObject.SetActive(!isRuntime);
            _runtimeObject.gameObject.SetActive(isRuntime);
        }
    }
}