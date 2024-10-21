using Game.Data;
using UnityEngine;

namespace Game.Input
{
    public interface IInputReceiver : IDataController
    {
        void ReceiveInput(Vector3 direction);
    }
}