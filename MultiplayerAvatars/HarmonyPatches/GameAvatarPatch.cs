using HarmonyLib;
using IPA.Utilities;
using MultiplayerAvatars.Avatars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiplayerAvatars.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerCoreInstaller), "InstallBindings", MethodType.Normal)]
    class GameAvatarPatch
    {
        static void Postfix(MultiplayerCoreInstaller __instance)
        {
            if (IPA.Loader.PluginManager.GetPluginFromId("CustomAvatar") != null)
            {
                MultiplayerPlayersManager playersManager = Resources.FindObjectsOfTypeAll<MultiplayerPlayersManager>().First();

                MultiplayerConnectedPlayerFacade playerFacade = playersManager.GetField<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>("_connectedPlayerControllerPrefab");
                MultiplayerConnectedPlayerFacade duelPlayerFacade = playersManager.GetField<MultiplayerConnectedPlayerFacade, MultiplayerPlayersManager>("_connectedPlayerDuelControllerPrefab");
                SetupPoseController(playerFacade);
                SetupPoseController(duelPlayerFacade);
            }
        }

        static void SetupPoseController(MultiplayerConnectedPlayerFacade playerFacade)
        {
            MultiplayerAvatarPoseController multiplayerPoseController = playerFacade.GetComponentsInChildren<MultiplayerAvatarPoseController>().First();
            if (!multiplayerPoseController.gameObject.TryGetComponent<CustomAvatarController>(out CustomAvatarController controller))
            {
                multiplayerPoseController.gameObject.AddComponent<CustomAvatarController>();
            }
        }
    }
}
