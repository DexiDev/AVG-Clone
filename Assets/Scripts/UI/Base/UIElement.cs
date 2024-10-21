using System;
using Game.Assets;
using Game.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.UI
{
    public class UIElement : DataController, IAssetInstance
    {
        [SerializeField] private UIElementType _uiElementType;
        [field: SerializeField] public AssetGroupData AssetGroup { get; private set; }
        
        public IAssetContract Contract { get; set; }
        public GameObject Instance => gameObject;
        
        public event Action<IAssetInstance> OnPoolable;
        public event Action<IAsset> OnReleased;
        public UIElementType UIElementType => _uiElementType;

        protected virtual void Awake()
        {
            
        }
        
        protected virtual void OnDisable()
        {
            OnPoolable?.Invoke(this);
        }
        
        public virtual void OnShow(){}

        public virtual void OnHide()
        {
            if(!gameObject.IsDestroyed()) gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            OnReleased?.Invoke(this);
        }
    }
}