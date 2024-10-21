using Game.Data.Fields;
using Game.Unit.Damageable;
using Game.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerController : DamageableController
    {
        [SerializeField, TabGroup("Components")] private WeaponController _weaponController;
        [SerializeField, TabGroup("Parameters")] private float _acceleration;
        
        [ShowInInspector, TabGroup("Debug")] private float _currentSpeed;

        private IsBattleStateField _weaponIsBattleStateField;
        
        protected override void Awake()
        {
            _weaponIsBattleStateField = _weaponController.GetDataField<IsBattleStateField>(true);
            
            _weaponController.Initialize(this);
            
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnIsBattleStateFieldChanged(_isBattleStateField.Value);
            _isBattleStateField.OnChanged += OnIsBattleStateFieldChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _isBattleStateField.OnChanged -= OnIsBattleStateFieldChanged;
        }

        private void OnIsBattleStateFieldChanged(bool value)
        {
            _weaponIsBattleStateField.SetValue(value);

            if (!value) _currentSpeed = 0f;
        }

        protected override void Move(Vector3 direction, float speed)
        {
            base.Move(direction, _currentSpeed);
        }

        private void Update()
        {
            if (_isBattleStateField.Value)
            {
                _currentSpeed += _acceleration * Time.deltaTime;

                var speed = GetSpeed();
                _currentSpeed = Mathf.Clamp(_currentSpeed, -speed, speed);

                Move(Vector3.forward);
            }
        }
    }
}