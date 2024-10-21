
using UnityEngine;

namespace Game.Data.Fields.Moving
{
    public class MoveToTransformField : TransformField, IMoveField
    {
        [SerializeField] private float _stoppingDistance;
        [SerializeField] private bool _isAutoRemove;
        
        public float StoppingDistance => _stoppingDistance;
        
        public bool IsAutoRemove => _isAutoRemove;
        
        public MoveToTransformField(Transform value, float stoppingDistance = -1f, bool isAutoRemove = true)
        {
            _value = value;
            _stoppingDistance = stoppingDistance;
            _isAutoRemove = isAutoRemove;
        }

        public Vector3 GetPosition()
        {
            return Value.position;
        }

        public void SetAutoRemove(bool isAutoRemove)
        {
            _isAutoRemove = isAutoRemove;
            InvokeDataChanged();
        }

        public void SetStoppingDistance(float stoppingDistance)
        {
            _stoppingDistance = stoppingDistance;
            InvokeDataChanged();
        }
    }
}