using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Loadings.Installers
{
    public class LoadingManagerInstaller : IInstaller
    {
        [SerializeField] private LoadingConfig _config;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LoadingManager>().WithParameter(_config).AsSelf();
        }
    }
}