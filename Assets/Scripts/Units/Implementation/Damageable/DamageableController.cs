using System;
using Game.Data.Attributes.Fields;
using Game.Data.Fields;
using Game.Data.Fields.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Unit.Damageable
{
    [RequireDataField(typeof(HealthField))]
    [RequireDataField(typeof(IsBattleStateField))]
    public class DamageableController : UnitController, IDamageable
    {
        [SerializeField, TabGroup("Components")] private Collider _collider;
        
        protected HealthField _healthField;
        protected IsBattleStateField _isBattleStateField;

        public HealthField HealthField => _healthField;
        public Collider Collider => _collider;
        public IsBattleStateField IsBattleStateField => _isBattleStateField;

        public event Action<IDamageable> OnKill;
        public event Action<float> OnDamage;

        public bool IsAlive => _healthField.Value > _healthField.MinValue;
        
        protected override void Awake()
        {
            base.Awake();
            _healthField = GetDataField<HealthField>();
            _isBattleStateField = GetDataField<IsBattleStateField>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _healthField.ResetToBase();
        }
        
        public virtual void ReceiveDamage(IDamageable source, float count)
        {
            if (!IsAlive) return;

            bool isKill = IsCanKill(count);
            
            _healthField.DecreaseValue(count);
            
            OnDamage?.Invoke(count);
            if (isKill)
            {
                OnKill?.Invoke(this);
            }
        }
        
        public bool IsCanKill(float count)
        {
            return _healthField.Value - count <= _healthField.MinValue;
        }
    }
}