using UnityEngine;

namespace Game.Data.Fields.Moving
{
    public class MoveToPointField : Vector3Field, IMoveField
    {
        [SerializeField] private float _stoppingDistance;
        [SerializeField] private bool _isAutoRemove;
        
        public float StoppingDistance => _stoppingDistance;
        
        public bool IsAutoRemove => _isAutoRemove;
        
        public MoveToPointField(Vector3 value, float stoppingDistance = -1f, bool isAutoRemove = true)
        {
            _value = value;
            _stoppingDistance = stoppingDistance;
            _isAutoRemove = isAutoRemove;
        }

        public Vector3 GetPosition()
        {
            return Value;
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