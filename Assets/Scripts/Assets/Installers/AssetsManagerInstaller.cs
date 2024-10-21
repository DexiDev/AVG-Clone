using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Assets.Installers
{
    public class AssetsManagerInstaller : IInstaller
    {
        [SerializeField] private AssetConfig _config;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<AssetsManager>().WithParameter(_config).AsSelf();
        }
    }
}