using MultiplayerAvatars.Avatars;
using SiraUtil.Extras;
using SiraUtil.Objects.Multiplayer;
using UnityEngine;
using Zenject;

namespace MultiplayerAvatars.Installers
{
    internal class MpavGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.RegisterRedecorator(new ConnectedPlayerRegistration(DecorateConnectedPlayerFacade));
            Container.RegisterRedecorator(new ConnectedPlayerDuelRegistration(DecorateConnectedPlayerFacade));
        }

        private MultiplayerConnectedPlayerFacade DecorateConnectedPlayerFacade(MultiplayerConnectedPlayerFacade original)
        {
            original.GetComponentInChildren<MultiplayerAvatarPoseController>().gameObject.AddComponent<CustomAvatarController>();
            GameObject.Destroy(original.GetComponentInChildren<AvatarPoseController>().gameObject.GetComponent<Animator>());
            return original;
        }
    }
}
