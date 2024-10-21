using System;
using System.Collections.Generic;
using System.Linq;
using Game.Assets.Providers.Instantiates;
using Game.Data;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using Object = UnityEngine.Object;

namespace Game.Assets
{
    public class AssetsManager : DataManager<AssetGroupData, AssetConfig>
    {
        private static readonly Dictionary<Type, IAssetProvider> _assetsProviders = new()
        {
            { typeof(IAssetInstance), new InstantiateProvider() },
        };
        
        private readonly Dictionary<Type, IAssetProvider> _assetsProvidersInstance = new();
        
        private readonly HashSet<IAsset> _assetsActive = new();
        private readonly Dictionary<IAssetContract, HashSet<IAsset>> _assetsPool = new();
        
        private Transform _container;
        private IObjectResolver _objectResolver;

        [Inject]
        private void Install(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected override void Initialized()
        {
            base.Initialized();
            
            CreateContainer();
            
            foreach (var assetProvider in _assetsProviders)
            {
                _objectResolver.Inject(assetProvider.Value);

                _assetsProvidersInstance.Add(assetProvider.Key, assetProvider.Value);
                
                assetProvider.Value.Initialize(_container);
            }
        }

        private void CreateContainer()
        {
            var container = new GameObject("Asset Container")
            {
                active = false
            };
            
            Object.DontDestroyOnLoad(container);
            
            _container = container.transform;
        }

        public T GetAsset<T>(IAssetContract contract, Transform parent) where T : class, IAsset
        {
            if(parent != null) return GetAsset<T>(contract, parent.position, parent.rotation, parent);
            else return GetAsset<T>(contract, Vector3.zero, Quaternion.identity, parent);
        }

        public T GetAsset<T>(IAssetContract contract, Vector3 position, Quaternion rotation, Transform parent, Scene scene) where T : class, IAsset
        {
            var asset = GetAsset<T>(contract, position, rotation, parent);

            asset.GetGameObject();
            
            if (asset.TryGetGameObject(out var gameObject))
            {
                SceneManager.MoveGameObjectToScene(gameObject, scene);
            }

            return asset;
        }

        public T GetAsset<T>(IAssetContract contract, Vector3 position, Quaternion rotation, Transform parent) where T : class, IAsset
        {
            if (contract == null) return null;
            
            T asset = null;
            
            if(_assetsPool.TryGetValue(contract, out var poolList))
            {
                asset = poolList.FirstOrDefault() as T;
            }

            if (asset != null)
            {
                _assetsPool[contract].Remove(asset);

                if (asset is Object obj)
                {
                    if (obj.IsUnityNull() || obj.IsDestroyed())
                    {
                        asset = GetAsset<T>(contract, position, rotation, parent);
                        return asset;
                    }

                    var gm = obj.GameObject();
                    if (gm != null)
                    {
                        gm.transform.position = position;
                        gm.transform.rotation = rotation;
                        gm.transform.SetParent(parent, true);
                        gm.transform.SetAsLastSibling();
                        gm.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                IAssetProvider assetProvider = GetProvider(_assetsProvidersInstance, contract);

                if (assetProvider != null)
                {
                    asset = assetProvider.GetAsset<T>(contract, position, rotation);

                    asset.Contract = contract;

                    if (asset.TryGetGameObject(out var gameObject))
                    {
                        gameObject.transform.SetParent(parent, true);   
                    }

                    asset.OnReleased += OnAssetRelease;
                }
                else return null;
            }


            if (asset is IAssetInstance assetInstance)
            {
                assetInstance.OnPoolable += OnAssetPoolable;   
            }
            
            _assetsActive.Add(asset);
            
            return asset;
        }

        private TAssetProvider GetProvider<TAssetProvider>(Dictionary<Type, TAssetProvider> assetProviders, IAssetContract assetContract) where TAssetProvider : class, IAssetProvider
        {
            Type assetType = assetContract.GetType();
            Type closestType = null;
            TAssetProvider closestProvider = null;

            foreach (var kvp in assetProviders)
            {
                Type providerType = kvp.Key;
                if (providerType.IsAssignableFrom(assetType))
                {
                    if (closestType == null || closestType.IsAssignableFrom(providerType))
                    {
                        closestType = providerType;
                        closestProvider = kvp.Value;
                    }
                }
            }

            return closestProvider;
        }

        public void AssetPoolable(IAssetInstance assetInstance)
        {
            OnAssetPoolable(assetInstance);
        }

        private void OnAssetPoolable(IAssetInstance assetInstance)
        {
            if (!_assetsActive.Contains(assetInstance)) return;
            
            assetInstance.OnPoolable -= OnAssetPoolable;

            var assetContract = assetInstance.Contract;
            
            _assetsActive.Remove(assetInstance);
            
            if (!assetInstance.Instance.IsDestroyed())
            {
                if (assetInstance.AssetGroup != null)
                {
                    var count = _assetsPool?
                        .Where(poolAsset => poolAsset.Value?.Count > 0)
                        .SelectMany(poolAsset => poolAsset.Value)
                        .FilterCast<IAssetInstance>()
                        .Count(poolAsset => poolAsset.AssetGroup == assetInstance.AssetGroup);

                    if (assetInstance.AssetGroup.PoolLimit <= count)
                    {
                        Object.Destroy(assetInstance.Instance);
                        return;
                    }
                }
                
                _assetsPool.TryAdd(assetContract, new HashSet<IAsset>());

                _assetsPool[assetContract].Add(assetInstance);

                if (assetInstance.Instance.activeSelf)
                {
                    assetInstance.Instance.SetActive(false);
                }
            }
        }

        private void OnAssetRelease(IAsset asset)
        {
            if (asset == null) return;
            
            asset.OnReleased -= OnAssetRelease;

            IAssetProvider assetProvider = null;
            
            if (asset.Contract != null)
            {
                assetProvider = _assetsProviders.GetValueOrDefault(asset.Contract.GetType());
            }
            
            assetProvider?.OnAssetRelease(asset);
            
            foreach (var contractPool in _assetsPool)
            {
                if (contractPool.Value == null) continue;

                if (contractPool.Value.Remove(asset))
                {
                    if (contractPool.Value.Count > 0)
                    {
                        assetProvider?.OnContractRelease(asset.Contract);
                    }
                    return;
                }
            }
        }
    }
}