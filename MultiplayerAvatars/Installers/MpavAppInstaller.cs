using MultiplayerAvatars.Providers;
using Zenject;

namespace MultiplayerAvatars.Installers
{
    internal class MpavAppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CustomAvatarsProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<ModelSaberProvider>().AsSingle();
            Container.Bind<AvatarProviderService>().ToSelf().AsSingle();
        }
    }
}
