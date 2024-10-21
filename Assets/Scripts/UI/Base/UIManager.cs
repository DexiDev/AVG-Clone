using System.Collections.Generic;
using System.Linq;
using Game.Assets;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;

namespace Game.UI
{
    public class UIManager
    {
        private Dictionary<UIElementType, UIContainer> _uiContainers = new();
        private AssetsManager _assetsManager;

        private Dictionary<UIElement, List<UIElement>> _poolUIElements = new();
        
        [Inject]
        private void Construct(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        
        public void RegisterContainer(UIContainer uiContainer)
        {
            Transform oldContainer = null;

            if (_uiContainers.TryGetValue(uiContainer.ContainerType, out UIContainer cachedUIContainer))
            {
                oldContainer = cachedUIContainer.transform;
            }
            
            _poolUIElements.ForEach(contract =>
            {
                if (contract.Key.UIElementType == uiContainer.ContainerType)
                {
                    contract.Value.ForEach(uiElement =>
                    {
                        if (uiElement.transform.parent == oldContainer)
                            uiElement.transform.SetParent(uiContainer.transform, true);
                    });
                }
            });
            
            if (!_uiContainers.TryAdd(uiContainer.ContainerType, uiContainer))
            {
                _uiContainers[uiContainer.ContainerType] = uiContainer;
            }
        }

        public void UnregisterContainer(UIContainer uiContainer)
        {
            if (_uiContainers.TryGetValue(uiContainer.ContainerType, out UIContainer cachedUIContainer))
            {
                if (cachedUIContainer == uiContainer)
                {
                    _uiContainers.Remove(uiContainer.ContainerType);
                }
            }
        }

        public T ShowElement<T>(T uiContract, Transform parent = null) where T : UIElement
        {
            if (parent == null)
            {
                if(_uiContainers.TryGetValue(uiContract.UIElementType, out UIContainer cachedUIContainer))
                    parent = cachedUIContainer.transform;
                else if (_uiContainers.TryGetValue(UIElementType.Other, out UIContainer cachedUIContainerOther))
                    parent = cachedUIContainerOther.transform;
            }
            
            var asset = _assetsManager.GetAsset<T>(uiContract, parent);

            var rectTransform = asset.GetComponent<RectTransform>();
            var targetRectTransform = uiContract.GetComponent<RectTransform>();

            rectTransform.pivot = targetRectTransform.pivot;
            rectTransform.anchorMin = targetRectTransform.anchorMin;
            rectTransform.anchorMax = targetRectTransform.anchorMax;
            rectTransform.offsetMin = targetRectTransform.offsetMin;
            rectTransform.offsetMax = targetRectTransform.offsetMax;
            rectTransform.localPosition = targetRectTransform.localPosition;
            rectTransform.localScale = targetRectTransform.localScale;

            asset.OnPoolable += OnPoolableElement;

            _poolUIElements.TryAdd(uiContract, new());
            
            _poolUIElements[uiContract].Add(asset);

            asset.OnShow();

            return asset;
        }

        public void HideElement<T>(T uiElement = null) where T : UIElement
        {
            if (uiElement != null)
            {
                uiElement.OnHide();
            }
            // else
            // {
                // _poolUIElements.FirstOrDefault(contract => contract.Key.GetType() == typeof(T)).Value?
                //     .ForEach(element => element.OnHide());
            // }
        }

        private void OnPoolableElement(IAssetInstance assetInstance)
        {
            assetInstance.OnPoolable -= OnPoolableElement;

            var uiElement = assetInstance as UIElement;
            
            foreach (var contract in _poolUIElements)
            {
                if (contract.Value.Contains(uiElement))
                {
                    contract.Value.Remove(uiElement);
                    return;
                }
            }
        }

        public bool ContainsElement<T>(T uiElement = null) where T : UIElement
        {
            return uiElement != null ? _poolUIElements.ContainsKey(uiElement) && _poolUIElements[uiElement].Count > 0 : _poolUIElements.Any(contract => contract.Key is T && contract.Value.Count > 0);
        }

        public T[] GetElements<T>(T uiContract) where T : UIElement
        {
            if (!ContainsElement<T>(uiContract)) return null;

            return _poolUIElements[uiContract].Cast<T>().ToArray();
        }

        public T GetElement<T>(T uiContract) where T : UIElement
        {
            return GetElements<T>(uiContract)?.First();
        }
        
        // public IEnumerable<UIElement> GetActiveUIElements()
        // {
        //     return _poolUIElements.SelectMany(contract => contract.Value);
        // }
    }
}