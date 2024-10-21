using Game.Assets;
using UnityEngine;
using VContainer;

namespace Game.UI.UIElements
{
    public class UIButtonElementActive : UIButton
    {
        [SerializeField] protected UIElement _uiElement;
        [SerializeField] protected bool _isActive;

        protected UIManager _uiManager;
        protected UIElement _uiElementInstance;
        
        [Inject]
        private void Install(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        protected override void OnDestroy()
        {
            if (_uiElementInstance != null) OnPoolableUIElement(_uiElementInstance);
            base.OnDestroy();
        }

        protected override void OnClickHandler()
        {
            base.OnClickHandler();

            if (_isActive)
            {
                if (_uiElementInstance == null)
                {
                    _uiElementInstance = _uiManager.ShowElement(_uiElement);
                    _uiElementInstance.OnPoolable += OnPoolableUIElement;
                }
            }
            else
            {
                _uiManager.HideElement(_uiElement);
            }
        }

        private void OnPoolableUIElement(IAssetInstance assetInstance)
        {
            _uiElementInstance.OnPoolable -= OnPoolableUIElement;
            _uiElementInstance = null;
        }
    }
}