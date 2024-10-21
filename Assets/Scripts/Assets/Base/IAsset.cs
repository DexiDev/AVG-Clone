using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Assets
{
    public interface IAsset
    {
        IAssetContract Contract { get; set; }
        
        event Action<IAsset> OnReleased;

        GameObject GetGameObject()
        {
            if (this is Object obj)
            {
                var gameObject = obj.GameObject();
                if (gameObject != null)
                {
                    return gameObject;
                }
            }
            
            return null;
        }


        bool TryGetGameObject(out GameObject gameObject)
        {
            gameObject = GetGameObject();

            return gameObject != null;
        }
    }
    
    public interface IAsset<out TInstance> : IAsset
    {
        TInstance Instance { get; }
    }
}