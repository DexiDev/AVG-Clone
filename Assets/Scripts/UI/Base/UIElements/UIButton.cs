using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIButton : UIElement
    {
        [SerializeField] protected Button _button;
        
        public Button Button => _button;
        
        public event Action OnClick;
        
        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(OnClickHandler);
        }

        protected override void OnDisable()
        {
            _button.onClick.RemoveListener(OnClickHandler);
            base.OnDisable();
        }

        protected virtual void OnClickHandler()
        {
            OnClick?.Invoke();
        }
    }
}