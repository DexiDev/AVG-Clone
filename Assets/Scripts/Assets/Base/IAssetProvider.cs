using System;
using UnityEngine;

namespace Game.Assets
{
    public interface IAssetProvider
    {
        
        void Initialize(Transform container);
        
        Type GetTypeData();
        
        void OnAssetRelease(IAsset asset);

        void OnContractRelease(IAssetContract assetContract);
        
        /// <summary>
        /// Gets an asset synchronously.
        /// </summary>
        /// <typeparam name="T">The type of the asset.</typeparam>
        /// <param name="contract">The asset contract.</param>
        /// <param name="position">The position to place the asset.</param>
        /// <param name="rotation">The rotation to apply to the asset.</param>
        /// <returns>The asset.</returns>
        T GetAsset<T>(IAssetContract contract, Vector3 position, Quaternion rotation) where T : class, IAsset;
    }
}