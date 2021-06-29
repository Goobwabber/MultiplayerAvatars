using MultiplayerAvatars.Avatars;
using MultiplayerAvatars.Providers;
using Zenject;

namespace MultiplayerAvatars.Installers
{
    class AvatarInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log?.Info("Injecting Dependencies");
            Container.BindInterfacesAndSelfTo<ModelSaber>().AsSingle();
            Container.BindInterfacesAndSelfTo<CustomAvatarManager>().AsSingle();
        }
    }
}
