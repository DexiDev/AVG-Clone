using System;
using Game.Data.Attributes.Fields;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data.Fields.Stats
{
    [UniqueDataField]
    public class HealthField : FloatField, IBaseValue<float>, IMinMaxValue<float>, IStatField
    {
        [NonSerialized, ReadOnly, ShowInInspector] protected new float _value;
        
        [SerializeField, OnValueChanged(nameof(ResetToBase))] private float _baseValue;
        
        [field: SerializeField] public Sprite Icon { get; private set; }
        
        [field: SerializeField] public string Name { get; private set; }
        
        public override float Value
        {
            get => _value;
            protected set => _value = value;
        }
        
        public virtual float BaseValue
        {
            get => _baseValue;
            protected set => _baseValue = value;
        }
        
        public virtual float MinValue => 0;
        public virtual float MaxValue => _baseValue;

        public override void SetValue(float value)
        {
            var newValue = ClampValue(value);
            base.SetValue(newValue);
        }

        public override void SetInstance(IDataField dataField)
        {
            if (dataField is IBaseValue<float> baseValue)
            {
                if (!BaseValue.Equals(baseValue.BaseValue))
                {
                    BaseValue = baseValue.BaseValue;
                }
            }
            
            base.SetInstance(dataField);
        }

        public float ClampValue(float value)
        {
            return Mathf.Clamp(value, MinValue, MaxValue);
        }
        
        public string ValueToStringUI()
        {
            return  $"{BaseValue.ToString()} HP";
        }
        
        public string ValueToString()
        {
            return Value.ToString();
        }
        
        public void ResetToBase()
        {
            SetValue(BaseValue);
        }
    }
}