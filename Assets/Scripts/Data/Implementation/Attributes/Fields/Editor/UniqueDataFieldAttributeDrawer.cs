#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;

namespace Game.Data.Attributes.Fields
{
    public class UniqueDataFieldAttributeDrawer
    {
        public static void OnInspectorGUI(DataController dataController)
        {
            var dataFields = dataController.GetDataFields<IDataField>();

            if (dataFields != null)
            {
                foreach (var field in dataFields)
                {
                    var fieldType = field.GetType();
                    var isUnique = Attribute.IsDefined(fieldType, typeof(UniqueDataFieldAttribute));

                    if (isUnique)
                    {
                        var targetDataFields = dataFields.Where(dataField => dataField.GetType() == fieldType).ToList();

                        if (targetDataFields.Count() > 1)
                        {
                            targetDataFields.Remove(field);

                            foreach (var targetDataField in targetDataFields)
                            {
                                dataController.RemoveDataField(targetDataField);
                            }

                            EditorUtility.DisplayDialog("Error",
                                $"Field of type {fieldType.Name} can only have one instance. Duplicates have been removed.",
                                "OK");
                            return;
                        }
                    }
                }
            }
        }
    }
}
#endif