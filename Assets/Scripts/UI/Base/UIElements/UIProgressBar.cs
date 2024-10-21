using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.UIElements
{
    public class UIProgressBar : UIElement
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private TMP_Text _textValue;
        
        public void SetValue(float value)
        {
            _progressBar.fillAmount = value;
            if(_textValue != null) _textValue.text = ((int)(value * 100f)).ToString();
        }
    }
}