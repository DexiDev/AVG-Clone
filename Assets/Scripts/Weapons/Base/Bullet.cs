using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Assets.Assets;
using Game.Data;
using Game.Data.Attributes.Fields;
using Game.Data.Fields.Stats;
using Game.Renderers;
using Game.Unit.Damageable;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Weapons
{
    [RequireDataField(typeof(DamagePowerField))]
    public class Bullet : DataController, IAssetInstance
    {
        [SerializeField, TabGroup("Parameters")] private AssetGroupData _assetGroupData;
        [SerializeField, TabGroup("Parameters")] private float _speed;
        [SerializeField, TabGroup("Parameters")] private float _acceleration;
        [SerializeField, TabGroup("Parameters")] private LayerMask _layerShoot;
        [SerializeField, TabGroup("Effects")] private ParticleAsset _explodeParticleContract;
        [SerializeField, TabGroup("Components")] private RendererVisible[] _rendererVisibles;
        [SerializeField, TabGroup("Components")] private TrailRenderer _trailRenderer;
        
        [ShowInInspector] private float _currentSpeed;
        
        private AssetsManager _assetsManager;
        private DamagePowerField _damagePowerField;
        private bool _isMoving;
        
        private IDamageable _damageableSource;
        private RaycastHit[] _cachedHits = new RaycastHit[1];
        private CancellationTokenSource _cancellationToken;
        
        public AssetGroupData AssetGroup => _assetGroupData;
        public GameObject Instance => gameObject;
        public IAssetContract Contract { get; set; }
        
        public event Action<IAssetInstance> OnPoolable;
        public event Action<IAsset> OnReleased;

        [Inject]
        private void Install(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        private void Awake()
        {
            _damagePowerField = GetDataField<DamagePowerField>(true);
        }

        public void Initialize(IDamageable source)
        {
            _damageableSource = source;
        }
        
        private void OnEnable()
        {
            _isMoving = true;
            _currentSpeed = _speed;

            if (_rendererVisibles != null)
            {
                foreach (var rendererVisible in _rendererVisibles)
                {
                    rendererVisible.OnVisible += OnRendererVisible;
                }
            }
        }

        private void OnDisable()
        {
            if (_rendererVisibles != null)
            {
                foreach (var rendererVisible in _rendererVisibles)
                {
                    rendererVisible.OnVisible -= OnRendererVisible;
                }
            }
            _trailRenderer.Clear();
            OnPoolable?.Invoke(this);
        }

        private void OnDestroy()
        {
            _cancellationToken?.Cancel();
            OnReleased?.Invoke(this);
        }
        
        private void OnRendererVisible(bool isVisible)
        {
            if(!isVisible && _rendererVisibles.All(rendererVisible => !rendererVisible.IsVisible)) gameObject.SetActive(false);
        }
        
        protected virtual void LateUpdate()
        {
            if (!_isMoving) return;
            var newPosition = transform.position + transform.forward * _currentSpeed * Time.deltaTime;
            var direction = (newPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, newPosition);

            Debug.DrawRay(transform.position, direction * distance, Color.red, Time.deltaTime);
            if (Physics.RaycastNonAlloc(transform.position, direction, _cachedHits, distance, _layerShoot) > 0)
            {
                if (_cachedHits[0].transform.TryGetComponent(out IDamageable damageable))
                {
                    transform.position = _cachedHits[0].point;
                    _isMoving = false;
                    DelayExplode(damageable);
                    return;
                }
            }
            
            transform.position = newPosition;
            _currentSpeed += _acceleration * Time.deltaTime;
        }

        private async void DelayExplode(IDamageable damageable)
        {
            _cancellationToken?.Cancel();
            _cancellationToken = new();

            var cancellationToken = _cancellationToken.Token;
            try
            {
                await UniTask.Yield(cancellationToken: cancellationToken);
                if (!cancellationToken.IsCancellationRequested && damageable.IsAlive)
                {
                    damageable.ReceiveDamage(_damageableSource, _damagePowerField.Value);
                    Explode();
                }
            }
            catch(OperationCanceledException){}
        }

        [Button]
        public void Explode()
        {
            if (_explodeParticleContract != null)
            {
                _assetsManager.GetAsset<ParticleAsset>(_explodeParticleContract, transform.position, Quaternion.identity, null);
            }
            
            gameObject.SetActive(false);
        }
    }
}