using VContainer;
using VContainer.Unity;

namespace Game.Input.Installers
{
    public class InputManagerInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InputManager>().AsSelf();
        }
    }
}