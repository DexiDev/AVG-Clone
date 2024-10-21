using Game.UI;
using UnityEngine;
using VContainer;

namespace Game.Levels.UI
{
    public class UILevelCompletedScreen : UIScreen
    {
        [SerializeField] private bool _isWin;
        [SerializeField] private UIButton _okButton;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _showAnimationName = "OnShow"; 
        
        private LevelManager _levelManager;
        
        private LevelData _levelData;

        [Inject]
        private void Install(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        
        public void Initialize(LevelData levelData)
        {
            _levelData = levelData;
        }

        public override void OnShow()
        {
            _animator.SetTrigger(_showAnimationName);
            base.OnShow();
        }

        private void OnEnable()
        {
            _okButton.OnClick += OnOkClick;
        }

        protected override void OnDisable()
        {
            _okButton.OnClick -= OnOkClick;
            base.OnDisable();
        }

        private void OnOkClick()
        {
            _levelManager.LoadCurrentLevel();
        }
    }
}