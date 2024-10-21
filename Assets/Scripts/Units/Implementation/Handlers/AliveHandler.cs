using Game.Core;
using Game.Data.Fields.Stats;
using Game.Unit.Damageable;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Unit.Handlers
{
    public class AliveHandler : IHandler<IDamageable>
    {
        [SerializeField] private Behaviour[] _componentsActive;
        [SerializeField] private Collider[] _colidersActive;
        
        private HealthField _healthField;
        private bool _isAlive;
        
        private void Awake()
        {
            _healthField = _targetData.GetDataField<HealthField>();
        }

        private void OnEnable()
        {
            _healthField.OnChanged += OnHealthChanged;
            OnHealthChanged(_healthField.Value);
        }

        private void OnDisable()
        {
            _healthField.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float value)
        {
            if (_isAlive != _targetData.IsAlive)
            {
                _isAlive = _targetData.IsAlive;
                _componentsActive.ForEach(component => component.enabled = _isAlive);
                _colidersActive.ForEach(component => component.enabled = _isAlive);
            }
            
        }
    }
}