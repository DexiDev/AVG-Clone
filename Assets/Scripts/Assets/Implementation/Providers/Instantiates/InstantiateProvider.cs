using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Game.Assets.Providers.Instantiates
{
    public class InstantiateProvider : IAssetProvider
    {
        [Inject] private IObjectResolver _objectResolver;

        private Transform _container;

        public void Initialize(Transform container)
        {
            _container = container;
        }

        public Type GetTypeData()
        {
            return typeof(IAssetInstance);
        }

        public T GetAsset<T>(IAssetContract contract, Vector3 position, Quaternion rotation) where T : class, IAsset
        {
            if (contract is Object obj)
            {
                // _objectResolver.InstantiatePrefabForComponent<T>(contract, position, rotation, _container);
                
                var assetInstance = Object.Instantiate(obj,position, rotation, _container);

                if (assetInstance is T tInstance)
                {
                    _objectResolver.InjectGameObject(tInstance.GetGameObject());
                    return tInstance;
                }
                else if (assetInstance is GameObject gameObject)
                {
                    _objectResolver.InjectGameObject(gameObject);
                    
                    var asset = gameObject.GetComponent<T>();

                    return asset;
                }
                else
                {
                    _objectResolver.Inject(assetInstance);
                }
            }

            return null;
        }

        public void OnAssetRelease(IAsset asset)
        {
            if (asset is IAssetInstance assetInstance)
            {
                if (assetInstance.Instance != null && !assetInstance.Instance.IsDestroyed())
                {
                    Object.Destroy(assetInstance.Instance);
                }
            }
        }

        public void OnContractRelease(IAssetContract assetContract)
        {
            
        }

        public UniTask<T> GetAssetAsync<T>(IAssetContract contract, Vector3 position, Quaternion rotation, CancellationToken cancellationToken) where T : class, IAsset
        {
            return new UniTask<T>(GetAsset<T>(contract, position, rotation));
        }
    }
}