using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data.Attributes.Fields;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Data
{
    public abstract class DataController : SerializedMonoBehaviour, IDataController
    {
        [OdinSerialize] private List<IDataField> _dataFields;

        public event Action<IDataField> OnFieldAdded;
        public event Action<IDataField> OnFieldRemoved;
        public event Action<IDataField, bool> OnFieldsChanged;

        public bool HasData<T>() where T : class, IDataField
        {
            if (_dataFields != null)
            {
                return _dataFields.Any(field => field.GetType() == typeof(T));
            }
            return false;
        }
        
        public bool HasData(Type dataFieldType)
        {
            if (_dataFields != null)
            {
                return _dataFields.Any(field => field.GetType() == dataFieldType);
            }
            return false;
        }
        
        public T[] GetDataFields<T>() where T : IDataField
        {
            return _dataFields?.FilterCast<T>().ToArray();
            //return (T[])_dataFields.Where(field => field is T).ToArray();
        }
        
        public T GetDataField<T>(T dataField = null) where T : class, IDataField
        {
            if (_dataFields != null)
            {
                Type targetType = ReferenceEquals(null, dataField) ? typeof(T) : dataField.GetType();
                T targetDataField = _dataFields.FirstOrDefault(field => field.GetType() == targetType) as T;

                return targetDataField;
            }

            return null;
        }
        
        public T GetDataField<T>() where T : IDataField
        {
            if (_dataFields != null)
            {
                Type targetType = typeof(T);

                var data = _dataFields.FirstOrDefault(field => field.GetType() == targetType);
                
                return (T)data;
            }

            return default;
        }
        
        public T GetDataField<T>(bool isAutoCreate, T dataField = null) where T : class, IDataField, new()
        {
            T targetDataField = GetDataField<T>(dataField);
            
            if (isAutoCreate && ReferenceEquals(null, targetDataField))
            {
                targetDataField = new T();
                AddDataField(targetDataField);
            }

            return targetDataField;
        }

        public bool TryGetDataField<T>(out T dataField) where T : class, IDataField, new()
        {
            dataField = GetDataField<T>();
            
            return dataField != null;
        }


        public bool TryAddDataField<T>(T dataField = null) where T : class, IDataField, new()
        {
            if (HasData<T>()) return false;
            
            AddDataField<T>(dataField);
            return true;
        }
        
        public void AddDataField<T>(T dataField) where T : class, IDataField
        {
            _dataFields ??= new List<IDataField>();
            
            var isUnique = Attribute.IsDefined(typeof(T), typeof(UniqueDataFieldAttribute));
            if (isUnique && HasData<T>())
            {
                Debug.LogError($"Cannot add another instance of {typeof(T).Name}. Only one instance is allowed.");
                return;
            }
            
            _dataFields.Add(dataField);
            OnFieldAdded?.Invoke(dataField);
            OnFieldsChanged?.Invoke(dataField, true);
        }
        
        public void RemoveDataField<T>(T dataField) where T : class, IDataField
        {
            if (_dataFields != null)
            {
                if (_dataFields.Remove(dataField))
                {
                    OnFieldRemoved?.Invoke(dataField);
                    OnFieldsChanged?.Invoke(dataField, false);
                }
            }
        }
    }
}