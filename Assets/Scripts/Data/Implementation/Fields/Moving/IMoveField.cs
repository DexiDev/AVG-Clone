using UnityEngine;

namespace Game.Data.Fields.Moving
{
    public interface IMoveField : IDataField
    {
        public float StoppingDistance { get; }
        public bool IsAutoRemove { get; }
        public Vector3 GetPosition();
        public void SetAutoRemove(bool isAutoRemove);
        
        public void SetStoppingDistance(float stoppingDistance);
    }
}