#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Game.Data.Attributes
{
    [CustomPropertyDrawer(typeof(DataIDAttribute))]
    public class DataIDAttributeDrawer : PropertyDrawer
    {
        private static Dictionary<Type, DataConfig> _instancesConfig = new();
        
        private GUIStyle GetGUIColor(Color color)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = color;
            style.active.textColor = color;
            style.focused.textColor = color;
            style.hover.textColor = color;
            return style;
        }
        // private GUIStyle GetDropdownStyle()
        // {
        //     GUIStyle style = EditorStyles.popup;
        //     return style;
        // }


        private string[] GetConfigIds(Type keyDataType, Type itemDataType)
        {
            try
            {
                DataConfig dataConfig = _instancesConfig.TryGetValue(keyDataType, out dataConfig) ? dataConfig : null;

                if (dataConfig == null)
                {
                    var guid = UnityEditor.AssetDatabase.FindAssets("t:" + keyDataType.Name).FirstOrDefault();

                    if (guid != null)
                    {
                        string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        dataConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<DataConfig>(assetPath);
                    }

                    if (!_instancesConfig.TryAdd(keyDataType, dataConfig)) _instancesConfig[keyDataType] = dataConfig;
                }

                if (dataConfig == null) throw new Exception($"Not found data config {keyDataType.Name}");

                return dataConfig.SystemData.Where(data => data.GetType().IsCastableTo(itemDataType)).Select(x => x.ID).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                List<string> items = new();
                
                DataIDAttribute dataIDAttribute = (DataIDAttribute)attribute;

                var configIds = GetConfigIds(dataIDAttribute.ConfigDataType, dataIDAttribute.ItemDataType);
                
                if(configIds != null) items.AddRange(configIds);
                
                int selectedID = -1;

                var selectValue = property.stringValue;
                
                if (items.Contains(selectValue))
                    selectedID = items.IndexOf(selectValue);
                
                int newSelectedID = EditorGUI.Popup(position, property.displayName, selectedID, items.ToArray(), EditorStyles.popup);

                if (newSelectedID != selectedID)
                {
                    property.stringValue = items[newSelectedID];

                    EditorUtility.SetDirty(property.serializedObject.targetObject);//repaint
                }
                
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}

#endif