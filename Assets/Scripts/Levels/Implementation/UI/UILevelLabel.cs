using Game.UI;
using TMPro;
using UnityEngine;
using VContainer;

namespace Game.Levels.UI
{
    public class UILevelLabel : UIElement
    {
        [SerializeField] private TMP_Text _textField;
        [SerializeField] private string _defaultText = "Level ";
        
        private LevelManager _levelManager;
        
        [Inject]
        private void Install(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void OnEnable()
        {
            _textField.text = $"{_defaultText}{_levelManager.CurrentLevelLoop + 1}";
        }
    }
}