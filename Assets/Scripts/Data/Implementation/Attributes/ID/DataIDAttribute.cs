using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Data.Attributes
{
    //example [SerializeField, DataID(typeof(ItemsConfig))] private string _itemID;
    //example [SerializeField, DataID(typeof(ItemsConfig), typeof(ItemDataScriptable)] private string _itemID;
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    [Conditional("UNITY_EDITOR")]
    public class DataIDAttribute : PropertyAttribute
    {
        private Type _configDataType;
        private Type _itemDataType;
        
        public Type ConfigDataType => _configDataType;
        public Type ItemDataType => _itemDataType;
        
        /// <summary>
        /// Get a list of IDs from ConfigData.
        /// </summary>
        /// <param name="configDataType"> Config containing Date ID.</param>
        public DataIDAttribute(Type configDataType)
        {
            _configDataType = configDataType;
            _itemDataType = typeof(DataScriptable);
        }

        /// <summary>
        /// Get a list of IDs from ConfigData.
        /// </summary>
        /// <param name="configDataType"> Config containing Date ID.</param>
        /// <param name="itemDataType"> Filter by DataScriptable type.</param>
        public DataIDAttribute(Type configDataType, Type itemDataType)
        {
            _configDataType = configDataType;
            _itemDataType = itemDataType;
        }
    }
}