using Game.UI;
using Game.UI.UIElements;
using UnityEngine;
using VContainer;

namespace Game.Levels.UI
{
    public class UILevelProgress : UIElement
    {
        [SerializeField] private UIProgressBar _progressBar;

        private LevelManager _levelManager;

        private LevelController _levelController;
        
        [Inject]
        private void Install(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        private void OnEnable()
        {
            SetLevelController(_levelManager.LevelController);
            _levelManager.OnControllerChanged += ControllerChanged;
        }

        protected override void OnDisable()
        {
            SetLevelController(null);
            _levelManager.OnControllerChanged -= ControllerChanged;
            base.OnDisable();
        }
        
        private void ControllerChanged(LevelController levelController)
        {
            _levelController = levelController;
        }

        private void SetLevelController(LevelController levelController)
        {
            if (_levelController != null)
            {
                _levelController.OnStart -= OnLevelStart;
            }

            _levelController = levelController;

            if (_levelController != null)
            {
                _progressBar.gameObject.SetActive(_levelController.IsRunning);
                
                var progress = _levelController.GetProgress();
                _progressBar.SetValue(progress);
                
                _levelController.OnStart += OnLevelStart;
            }
        }

        private void OnLevelStart()
        {
            _progressBar.gameObject.SetActive(true);
        }

        private void LateUpdate()
        {
            if (_levelController == null || !_levelController.IsRunning) return;
         
            var progress = _levelController.GetProgress();
            _progressBar.SetValue(progress);
        }
    }
}