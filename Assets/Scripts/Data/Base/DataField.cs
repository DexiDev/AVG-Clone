using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data
{
    public interface IDataField
    {
        public event Action<IDataField> OnDataChanged;
        
        // void SetValue(object value);
        
        void SetInstance(IDataField dataField);
    }

    public interface IDataField<T> : IDataField
    {
        public T Value { get; }
        
        public event Action<T> OnChanged;

        public void SetValue(T value);
    }
    
    [Serializable]
    public abstract class DataField<T> : IDataField<T>
    {
        [SerializeField, OnValueChanged(nameof(OnValueChanged))] protected T _value;

        public virtual T Value
        {
            get => _value;
            protected set => _value = value;
        }

        public event Action<T> OnChanged;
        public event Action<IDataField> OnDataChanged;
        
        public virtual void SetValue(T value)
        {
            if (!Equals(Value, value))
            {
                Value = value;
                OnChanged?.Invoke(value);
                InvokeDataChanged();
            }
        }

        protected virtual void OnValueChanged(T value)
        {
            OnChanged?.Invoke(value);
            InvokeDataChanged();
        }

        protected void InvokeDataChanged()
        {
            OnDataChanged?.Invoke(this);
        }

        protected virtual bool Equals(T oldValue, T newValue)
        {
            return oldValue == null && newValue == null || oldValue != null && oldValue.Equals(newValue);
        }

        protected virtual void SetValue(object value)
        {
            if (value is T tValue)
            {
                SetValue(tValue);
            }
        }

        public virtual void SetInstance(IDataField dataField)
        {
            if (dataField is DataField<T> data)
            {
                if (Value == null && data.Value != null || !Value.Equals(data.Value))
                {
                    SetValue(data.Value);
                }
            }
        }
    }
}