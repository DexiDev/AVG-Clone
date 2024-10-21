using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data.Fields
{
    public abstract class DictionaryField<TKey,TValue> : DataField<Dictionary<TKey, TValue>>
    {
        [SerializeField, OnValueChanged(nameof(OnValueChanged))] protected new Dictionary<TKey, TValue> _value = new();
        
        public override Dictionary<TKey, TValue> Value
        {
            get => _value ??= new();
            protected set => _value = value;
        }
        
        public event Action<TKey, TValue> OnItemAdded;
        
        public event Action<TKey, TValue> OnItemRemoved;

        public event Action<TKey, TValue> OnItemChanged;
        
        public void Add(TKey key, TValue item)
        {
            Value ??= new();
            
            Value.TryAdd(key, item);
            
            OnItemAdded?.Invoke(key, item);
            OnValueChanged(Value);
        }

        public void Remove(TKey key)
        {
            if(Value.Remove(key, out TValue value))
            {
                OnItemRemoved?.Invoke(key, value);
                OnValueChanged(Value);   
            }
        }

        public void SetValue(TKey key, TValue value)
        {
            Value ??= new();

            if (!Value.TryAdd(key, value))
            {
                Value[key] = value;
            }
            OnItemChanged?.Invoke(key, value);
            OnValueChanged(Value);
        }

        public void Clear()
        {
            Value.Clear();
            OnValueChanged(Value);
        }
    }
}