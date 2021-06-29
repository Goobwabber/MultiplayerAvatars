using IPA.Utilities;
using MultiplayerAvatars.Avatars;
using Zenject;

namespace MultiplayerAvatars.Installers
{
	class GameAvatarInstaller : Installer
	{
		public override void InstallBindings()
		{
			MultiplayerPlayersManager playersManager = Container.Resolve<MultiplayerPlayersManager>();
			playersManager.GetField<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>("_connectedPlayerControllerPrefab").GetComponentInChildren<MultiplayerAvatarPoseController>().gameObject.AddComponent<CustomAvatarController>();
			playersManager.GetField<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>("_connectedPlayerDuelControllerPrefab").GetComponentInChildren<MultiplayerAvatarPoseController>().gameObject.AddComponent<CustomAvatarController>();
		}
	}
}
