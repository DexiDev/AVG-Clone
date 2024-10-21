using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Data.Fields
{
    public abstract class ListField<T> : DataField<List<T>>
    {
        [SerializeField, OnValueChanged(nameof(OnValueChanged))] protected new List<T> _value = new();
        
        public override List<T> Value
        {
            get => _value ??= new();
            protected set => _value = value;
        }
        
        public event Action<T> OnItemAdded;
        
        public event Action<T> OnItemRemoved;
        
        public void Add(T item)
        {
            Value ??= new();

            Value.Add(item);
            
            OnItemAdded?.Invoke(item);
            OnValueChanged(Value);
        }

        public void Remove(T item)
        {
            Value.Remove(item);
            
            OnItemRemoved?.Invoke(item);
            OnValueChanged(Value);
        }

        public void Clear()
        {
            Value.Clear();
            OnValueChanged(Value);
        }

        public void Shuffle()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                var index = UnityEngine.Random.Range(i, Value.Count);
                var element = Value[index];
                
                Value.RemoveAt(index);
                Value.Insert(0, element);
            }
            
            OnValueChanged(Value);
        }
    }
}