using ControllThemAll.Runtime.MainMenu.UI;
using VContainer;
using VContainer.Unity;

namespace ControllThemAll.Runtime.MainMenu
{
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MainMenuUIFactory>(Lifetime.Singleton);
            builder.Register<MainMenuUIInstaller>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MainMenuEntryPoint>();
        }
    }
}
