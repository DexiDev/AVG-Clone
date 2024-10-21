using System;
using UnityEngine;

namespace Game.Assets
{
    public interface IAssetInstance : IAsset<GameObject>, IAssetContract
    {
        AssetGroupData AssetGroup { get; }
        
        event Action<IAssetInstance> OnPoolable;
    }
}