using HarmonyLib;
using IPA.Utilities;
using MultiplayerAvatars.Avatars;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MultiplayerAvatars.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerCoreInstaller), "InstallBindings", MethodType.Normal)]
    internal class GameAvatarPatch
    {
        internal static FieldAccessor<MultiplayerPlayersManager, MultiplayerConnectedPlayerFacade>.Accessor PlayerFacade = FieldAccessor<MultiplayerPlayersManager, MultiplayerConnectedPlayerFacade>.GetAccessor("_connectedPlayerControllerPrefab");
        internal static FieldAccessor<MultiplayerPlayersManager, MultiplayerConnectedPlayerFacade>.Accessor PlayerDuelFacade = FieldAccessor<MultiplayerPlayersManager, MultiplayerConnectedPlayerFacade>.GetAccessor("_connectedPlayerDuelControllerPrefab");

        internal static void Postfix(MultiplayerCoreInstaller __instance)
        {
            MonoInstallerBase mib = __instance;
            DiContainer container = SiraUtil.Accessors.GetDiContainer(ref mib);
            MultiplayerPlayersManager manager = container.Resolve<MultiplayerPlayersManager>();

            MultiplayerConnectedPlayerFacade playerFacade = PlayerFacade(ref manager);
            MultiplayerConnectedPlayerFacade duelPlayerFacade = PlayerDuelFacade(ref manager);
            SetupPoseController(playerFacade);
            SetupPoseController(duelPlayerFacade);
        }

        static void SetupPoseController(MultiplayerConnectedPlayerFacade playerFacade)
        {
            MultiplayerAvatarPoseController multiplayerPoseController = playerFacade.GetComponentInChildren<MultiplayerAvatarPoseController>();
            if (!multiplayerPoseController.gameObject.TryGetComponent(out CustomAvatarController _))
            {
                multiplayerPoseController.gameObject.AddComponent<CustomAvatarController>();
            }
        }
    }
}
