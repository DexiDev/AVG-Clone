using System.Collections.Generic;
using Game.Core;
using Game.Data.Fields;
using Game.Unit.Damageable;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Unit.Handlers
{
    public class BattleStateHandler : IHandler<DamageableController>
    {
        [SerializeField] private Dictionary<bool, Behaviour[]> _behavioursState;

        private IsBattleStateField _isBattleStateField;
        
        private void Awake()
        { 
            _isBattleStateField = _targetData.GetDataField<IsBattleStateField>(true);
        }

        private void OnEnable()
        {
            OnBattleStateChangedHandler(_isBattleStateField.Value);
            _isBattleStateField.OnChanged += OnBattleStateChangedHandler;
        }

        private void OnDisable()
        {
            _isBattleStateField.OnChanged -= OnBattleStateChangedHandler;
        }
        
        private void OnBattleStateChangedHandler(bool isBattleState)
        {
            if (_behavioursState == null) return;

            ToggleBehaviours(!isBattleState, false);
            ToggleBehaviours(isBattleState, true);
        }

        private void ToggleBehaviours(bool stateKey, bool enable)
        {
            if (_behavioursState.TryGetValue(stateKey, out var behaviours))
            {
                behaviours.ForEach(behaviour => behaviour.enabled = enable);
            }
        }
    }
}