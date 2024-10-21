using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Loadings.Boot
{
    [ShowOdinSerializedPropertiesInInspector]
    public class BootScope : LifetimeScope, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private List<IInstaller> _installers = new List<IInstaller>();
        
        [Inject]
        protected void Construct(IObjectResolver objectResolver)
        {
            _installers.ForEach(objectResolver.Inject);
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(gameObject);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            _installers.ForEach(installer => installer.Install(builder));
        }
        
        [SerializeField]
        [HideInInspector]
        private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData
        {
            get => this.serializationData;
            set => this.serializationData = value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject((Object) this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject((Object) this, ref this.serializationData);
        }

        /// <summary>Invoked after deserialization has taken place.</summary>
        protected virtual void OnAfterDeserialize()
        {
        }

        /// <summary>Invoked before serialization has taken place.</summary>
        protected virtual void OnBeforeSerialize()
        {
        }

#if UNITY_EDITOR
        [HideInTables]
        [OnInspectorGUI]
        [PropertyOrder(-2.147484E+09f)]
        private void InternalOnInspectorGUI() => EditorOnlyModeConfigUtility.InternalOnInspectorGUI((Object) this);
#endif
    }
}