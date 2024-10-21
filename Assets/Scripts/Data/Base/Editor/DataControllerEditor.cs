#if UNITY_EDITOR
using Game.Data.Attributes.Fields;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Game.Data
{
    
#if ODIN_INSPECTOR
    [CustomEditor(typeof(DataController), true)]
    public class DataControllerEditor : OdinEditor
#else
    [CustomEditor(typeof(DataController), true)]
    public class DataControllerEditor : Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (target is DataController dataController)
            {
                RequireDataFieldAttributeDrawer.OnInspectorGUI(dataController);
                UniqueDataFieldAttributeDrawer.OnInspectorGUI(dataController);
            }
        }
    }
}

#endif