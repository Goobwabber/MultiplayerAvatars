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
            Container.BindInterfacesAndSelfTo<ModelSaber>().AsSingle();
            Container.Bind(typeof(IInitializable), typeof(CustomAvatarManager)).To<CustomAvatarManager>().AsSingle();
        }
    }
}
