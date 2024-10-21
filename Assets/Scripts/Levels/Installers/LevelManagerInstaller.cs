using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Levels.Installers
{
    public class LevelManagerInstaller : IInstaller
    {
        [SerializeField] private LevelConfig _config;

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelManager>().WithParameter(_config).AsSelf();
        }
    }
}