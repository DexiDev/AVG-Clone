using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core
{
    public abstract class IHandler : SerializedMonoBehaviour
    {
        
    }
    
    public abstract class IHandler<T> : IHandler
    {
        [SerializeField] protected T _targetData;
    }
}