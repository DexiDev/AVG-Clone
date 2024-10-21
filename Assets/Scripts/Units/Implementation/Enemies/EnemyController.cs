using Game.Assets;
using Game.Assets.Assets;
using Game.Data.Attributes.Fields;
using Game.Data.Fields;
using Game.Data.Fields.Moving;
using Game.Data.Fields.Stats;
using Game.Unit.Damageable;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Game.Units.Enemies
{
    [RequireDataField(typeof(DamagePowerField))]
    [RequireDataField(typeof(IsBattleStateField))]
    public class EnemyController : DamageableController
    {
        [SerializeField, TabGroup("Animation")] private string _attackAnimation;
        [SerializeField, TabGroup("Parameters")] private float _runSpeed;
        [SerializeField, TabGroup("Parameters")] private float _distanceAttack;
        [SerializeField, TabGroup("VFX")] private ParticleAsset _particleDamage;
        [SerializeField, TabGroup("VFX")] private Transform _particlePoint;

        private AssetsManager _assetsManager;
        
        private IDamageable _target;
        private DamagePowerField _damagePowerField;

        [Inject]
        private void Install(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        
        protected override void Awake()
        {
            base.Awake();
            _damagePowerField = GetDataField<DamagePowerField>(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            OnBattleStateHandler(_isBattleStateField.Value);
            _isBattleStateField.OnChanged += OnBattleStateHandler;
        }

        protected override void OnDisable()
        {
            SetTarget(null);
            _isBattleStateField.OnChanged -= OnBattleStateHandler;
            base.OnDisable();
        }

        private void OnBattleStateHandler(bool value)
        {
            _animator.SetBool(_attackAnimation, value);
        }
        
        public void Agro(IDamageable target)
        {
            SetTarget(target);
            _isBattleStateField.SetValue(true);
        }

        public override void ReceiveDamage(IDamageable source, float count)
        {
            bool isCrit = Random.Range(0, 2) == 1;

            var damage = isCrit ? count * 2 : count;
            
            base.ReceiveDamage(source, damage);

            ShowParticle();
            if (IsAlive)
            {
                if(source != null) Agro(source);
            }
            else gameObject.SetActive(false);
        }

        public void SetTarget(IDamageable target)
        {
            if (_target == target) return;
            
            _target = target;

            if (target != null)
            {
                GetDataFields<IMoveField>()?.ForEach(RemoveDataField);
                
                var moveField = new MoveToTransformField(target.Instance.transform);
                AddDataField(moveField);
            }
            else
            {
                _isBattleStateField.SetValue(false);
            }
        }

        protected override float GetSpeed()
        {
            return _isBattleStateField.Value ? _runSpeed : base.GetSpeed();
        }

        private void LateUpdate()
        {
            if (_isBattleStateField.Value && IsAlive)
            {
                var point = _target.Collider.ClosestPoint(transform.position);

                if (Vector3.Distance(transform.position, point) <= _distanceAttack)
                {
                    Explode();
                }
            }
        }

        [Button]
        public void Explode()
        {
            _target?.ReceiveDamage(this, _damagePowerField.Value);
            ShowParticle();
            gameObject.SetActive(false);
        }

        private void ShowParticle()
        {
           _assetsManager.GetAsset<ParticleAsset>(_particleDamage, _particlePoint.position, Quaternion.identity, null);
        }
    }
}