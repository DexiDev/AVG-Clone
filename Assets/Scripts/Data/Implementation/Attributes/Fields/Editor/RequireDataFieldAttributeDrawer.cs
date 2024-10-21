#if UNITY_EDITOR

using System;
using UnityEngine;

namespace Game.Data.Attributes.Fields
{
    public class RequireDataFieldAttributeDrawer
    {
        public static void OnInspectorGUI(DataController dataController)
        {
            var controllerType = dataController.GetType();
            
            RequireDataFieldAttribute[] attributes = (RequireDataFieldAttribute[])Attribute.GetCustomAttributes(controllerType, typeof(RequireDataFieldAttribute));

            foreach (var attribute in attributes)
            {
                Type requiredFieldType = attribute.RequiredFieldType;

                if (!dataController.HasData(requiredFieldType))
                {
                    var newDataField = (IDataField)Activator.CreateInstance(requiredFieldType);
                    dataController.AddDataField(newDataField);

                    Debug.Log($"[DataController]: Automatically added missing required field of type {requiredFieldType.Name}.");
                }
            }
        }
    }
}
#endif