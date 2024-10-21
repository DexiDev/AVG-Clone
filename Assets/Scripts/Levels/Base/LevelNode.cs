using System;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Extensions.Collider;
using Game.Loadings;
using Game.Renderers;
using Game.Unit;
using Game.Units.Implementation.Fields;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Game.Levels
{
    public class LevelNode : SerializedMonoBehaviour, IAssetInstance
    {
        [SerializeField, TabGroup("Assets")] private AssetGroupData _assetGroupData;
        [SerializeField, TabGroup("Parameters")] private UnitController _unitContract;
        [SerializeField, TabGroup("Parameters")] private Vector2 _minMaxCount;
        [SerializeField, TabGroup("Components")] private BoxCollider _boxCollider;
        [SerializeField, TabGroup("Components")] private RendererVisible _rendererVisible;

        private AssetsManager _assetsManager;
        private LoadingManager _loadingManager;
        
        public bool IsVisible => _rendererVisible.IsVisible;
        public IAssetContract Contract { get; set; }
        public GameObject Instance => gameObject;
        public AssetGroupData AssetGroup => _assetGroupData;
        
        public event Action<IAssetInstance> OnPoolable;
        public event Action<IAsset> OnReleased;
        public event Action<LevelNode, bool> OnVisible;
        public event Action<UnitController> OnSpawned;

        [Inject]
        private void Install(AssetsManager assetsManager, LoadingManager loadingManager)
        {
            _assetsManager = assetsManager;
            _loadingManager = loadingManager;
        }

        private async void OnEnable()
        {
            await UniTask.WaitWhile(() => _loadingManager.IsLoading);

            _rendererVisible.OnVisible += OnRendererVisible;
        }

        private void OnDisable()
        {
            _rendererVisible.OnVisible -= OnRendererVisible;
            OnPoolable?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnReleased?.Invoke(this);
        }
        

        private void OnRendererVisible(bool isVisible)
        {
            OnVisible?.Invoke(this, isVisible);
        }

        public void SpawnAll()
        {
            var count = Random.Range(_minMaxCount.x, _minMaxCount.y);
            for (int i = 0; i < count; i++)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            Vector3 position = _boxCollider.bounds.GetRandomPoint();
            position.y = transform.position.y;

            var forwardDirection = position - transform.position;
            forwardDirection.Normalize();

            var rotation = Quaternion.LookRotation(forwardDirection);

            var unitController = _assetsManager.GetAsset<UnitController>(_unitContract, position, rotation, null);

            PatrolSpaceField patrolSpaceField = unitController.GetDataField<PatrolSpaceField>(true);
            
            patrolSpaceField.SetValue(_boxCollider);
            
            unitController.AddDataField(patrolSpaceField);
            
            OnSpawned?.Invoke(unitController);
        }
    }
}