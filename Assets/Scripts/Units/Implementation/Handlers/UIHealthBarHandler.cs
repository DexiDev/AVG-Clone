using Game.Assets;
using Game.Core;
using Game.Data.Fields.Moving;
using Game.Data.Fields.Stats;
using Game.UI;
using Game.UI.UIElements;
using Game.Unit.Damageable;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace Game.Unit.Handlers
{
    public class UIHealthBarHandler : IHandler<IDamageable>
    {
        [SerializeField] private UIProgressBar _uiElementContract;
        
        private UIManager _uiManager;

        private HealthField _healthField;
        
        private UIProgressBar _cachedUIElement;

        [Inject]
        private void Install(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        private void Awake()
        {
            _healthField = _targetData.GetDataField<HealthField>(true);
        }

        private void OnEnable()
        {
            OnDamageHandler(0);
            _targetData.OnDamage += OnDamageHandler;
        }

        private void OnDisable()
        {
            _targetData.OnDamage -= OnDamageHandler;
            if (_cachedUIElement != null) _uiManager.HideElement(_cachedUIElement);
        }
        

        private void OnDamageHandler(float value)
        {
            if (_healthField.Value < _healthField.BaseValue)
            {
                if (_cachedUIElement == null)
                {
                    _cachedUIElement = _uiManager.ShowElement(_uiElementContract);
                    _cachedUIElement.OnPoolable += OnUIElementPoolable;

                    _cachedUIElement.GetDataFields<IMoveField>()?.ForEach(moveField => _cachedUIElement.RemoveDataField(moveField));

                    var moveField = new MoveToTransformField(_targetData.Instance.transform);
                    _cachedUIElement.AddDataField(moveField);
                    
                    UpdateValue();
                }
            }
        }

        private void OnUIElementPoolable(IAssetInstance assetInstance)
        {
            assetInstance.OnPoolable -= OnUIElementPoolable;

            if (ReferenceEquals(assetInstance, _cachedUIElement))
            {
                _cachedUIElement = null;
            }
        }

        private void LateUpdate()
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            if (_cachedUIElement != null)
            {
                _cachedUIElement.SetValue(_healthField.Value / _healthField.BaseValue);
            }
        }
    }
}