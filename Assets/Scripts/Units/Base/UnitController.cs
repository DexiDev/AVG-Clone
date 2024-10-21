using System;
using Game.Assets;
using Game.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Unit
{
    [DisallowMultipleComponent]
    public class UnitController : DataController, IAssetInstance
    {
        [SerializeField, TabGroup("Parameters")] protected float _speed;
        [SerializeField, TabGroup("Parameters")] protected float _speedRotation;
        [SerializeField, TabGroup("Components")] protected Animator _animator;
        [SerializeField, TabGroup("Animation")] private string _isMovingKey = "IsMoving";
        [SerializeField, TabGroup("Animation")] private string _speedKey = "Speed";
        [SerializeField, TabGroup("Assets")] private AssetGroupData _assetGroupData;
        
        public IAssetContract Contract { get; set; }
        public GameObject Instance => gameObject;
        public Animator Animator => _animator;
        public AssetGroupData AssetGroup => _assetGroupData;
        public Transform Transform => transform;

        public event Action<Vector3> OnMove;
        public event Action<IAsset> OnReleased;
        public event Action<IAssetInstance> OnPoolable;
        
        protected virtual void Awake()
        {
            
        }

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
            OnPoolable?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            OnReleased?.Invoke(this);
        }

        public void Move(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Move(direction, GetSpeed());

            OnMove?.Invoke(direction);
        }

        public void MoveTowards(Vector3 targetPosition, float t = 1f)
        {
            var position = Vector3.MoveTowards(transform.position, targetPosition, GetSpeed() * t);
            var direction = (targetPosition - transform.position).normalized;
            transform.position = position;
            
            OnMove?.Invoke(direction);
        }

        protected virtual void Move(Vector3 direction, float speed)
        {
            var moveDirection = direction * speed; 
            transform.position += moveDirection;
        }
        
        public virtual void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            transform.localEulerAngles += direction * _speedRotation;
        }

        public virtual void RotateTowards(Vector3 direction, float t = 1f)
        {
            if (direction == Vector3.zero) return;
            
            var targetRotation = Quaternion.LookRotation(direction);

            var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,_speedRotation * t);
            
            SetRotation(rotation);
        }

        public virtual void SetRotation(Quaternion targetRotation)
        {
            transform.rotation = targetRotation;
        }
        
        public virtual void SetMotion(bool isMoving, float speed = 0f)
        {
            if (_animator == null) return;
            
            _animator.SetBool(_isMovingKey, isMoving);
            _animator.SetFloat(_speedKey, speed);
        }
        
        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        protected virtual float GetSpeed()
        {
            return _speed;
        }
    }
}