using MultiplayerAvatars.Avatars;
using MultiplayerAvatars.Downloaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerAvatars.Installers
{
    class AvatarInstaller : Installer
    {
        public override void InstallBindings()
        {
            Plugin.Log?.Info("Injecting Dependencies");

            if (IPA.Loader.PluginManager.GetPluginFromId("MultiplayerExtensions") != null)
            {
                Plugin.Log?.Info("Found MultiplayerExtensions");

                if (IPA.Loader.PluginManager.GetPluginFromId("CustomAvatar") != null)
                {
                    BindCustomAvatars(Container);
                }
            }
        }

        private void BindCustomAvatars(DiContainer Container)
        {
            Plugin.Log?.Info("Found CustomAvatar");
            Container.BindInterfacesAndSelfTo<ModelSaber>().AsSingle();
            Container.Bind(typeof(IInitializable), typeof(CustomAvatarManager)).To<CustomAvatarManager>().AsSingle();
        }
    }
}
