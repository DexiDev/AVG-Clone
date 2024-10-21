using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Assets;
using Game.Data;
using Game.Data.Attributes.Fields;
using Game.Data.Fields;
using Game.Data.Fields.Stats;
using Game.Input;
using Game.Unit.Damageable;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Weapons
{
    [RequireDataField(typeof(DamagePowerField))]
    [RequireDataField(typeof(DamageTickRateField))]
    [RequireDataField(typeof(IsBattleStateField))]
    public class WeaponController : DataController, IInputReceiver
    {
        [SerializeField, TabGroup("Components")] protected Animator _animator;
        [SerializeField, TabGroup("Animations")] private string _shootAnimation = "Shoot";
        [SerializeField, TabGroup("Parameters")] private Bullet _bulletContract;
        [SerializeField, TabGroup("Parameters")] private Transform _bulletStart;
        [SerializeField, TabGroup("Parameters")] private float _speedRotation;
        [SerializeField, TabGroup("Parameters")] private Vector2 _minMaxAngle = new Vector2(-45, 45f);


        private AssetsManager _assetsManager;

        private IsBattleStateField _isBattleStateField;
        private DamagePowerField _damagePowerField;
        private DamageTickRateField _damageTickRateField;
        private IDamageable _damageableSource;

        private CancellationTokenSource _cancellationTokenShoot;

        public event Action<Bullet> OnShoot;
        
        [Inject]
        private void Install(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        
        private void Awake()
        {
            _isBattleStateField = GetDataField<IsBattleStateField>();
            _damagePowerField = GetDataField<DamagePowerField>(true);
            _damageTickRateField = GetDataField<DamageTickRateField>(true);
        }

        public void Initialize(IDamageable source)
        {
            _damageableSource = source;
        }
        
        private void OnEnable()
        {
            OnIsBattleStateChange(_isBattleStateField.Value);
            _isBattleStateField.OnChanged += OnIsBattleStateChange;
        }

        private void OnDisable()
        {
            _cancellationTokenShoot?.Cancel();
            _isBattleStateField.OnChanged -= OnIsBattleStateChange;
        }
        
        private void OnIsBattleStateChange(bool isBattleState)
        {
            _cancellationTokenShoot?.Cancel();

            if (isBattleState)
            {
                ShootLoop();
            }
        }
        

        private async void ShootLoop()
        {
            _cancellationTokenShoot?.Cancel();
            _cancellationTokenShoot = new();

            var token = _cancellationTokenShoot.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    Shoot();
                    await UniTask.Delay(TimeSpan.FromSeconds(_damageTickRateField.Value), cancellationToken: token);
                }
            }
            catch(OperationCanceledException) {}
        }

        [Button]
        public void Shoot()
        {
            var bullet = _assetsManager.GetAsset<Bullet>(_bulletContract, _bulletStart.position, _bulletStart.rotation, null);

            if (bullet != null)
            {
                bullet.Initialize(_damageableSource);
                
                var damagePowerField = bullet.GetDataField<DamagePowerField>(true);
                damagePowerField.SetValue(_damagePowerField.Value);
            }
            if (_animator != null)
            {
                _animator.SetTrigger(_shootAnimation);
            }
            
            OnShoot?.Invoke(bullet);
        }

        public void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            transform.Rotate(direction * _speedRotation, Space.World);
            
            var currentRotation = transform.localEulerAngles;
            
            if (currentRotation.y > 180)
                currentRotation.y -= 360;
            currentRotation.y = Mathf.Clamp(currentRotation.y, _minMaxAngle.x,_minMaxAngle.y);
            
            transform.localEulerAngles = currentRotation;
        }

        public void ReceiveInput(Vector3 direction)
        {
            Rotate(direction);
        }
    }
}