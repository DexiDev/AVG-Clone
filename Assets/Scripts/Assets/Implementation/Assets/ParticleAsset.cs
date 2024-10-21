using System;
using UnityEngine;

namespace Game.Assets.Assets
{
    [DisallowMultipleComponent]
    public class ParticleAsset : MonoBehaviour, IAssetInstance
    {
        [SerializeField] private ParticleSystem _particle;
        [field: SerializeField] public AssetGroupData AssetGroup { get; private set; }
        
        public IAssetContract Contract { get; set; }
        
        public event Action<IAssetInstance> OnPoolable;
        public event Action<IAsset> OnReleased;

        public GameObject Instance => gameObject;

        private void OnDisable()
        {
            OnPoolable?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            OnReleased?.Invoke(this);
        }

        public void Play()
        {
            _particle.Play();
        }

        public void Stop()
        {
            _particle.Stop();
        }
    }
}