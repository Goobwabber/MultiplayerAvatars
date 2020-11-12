using HarmonyLib;
using IPA.Utilities;
using MultiplayerAvatars.Avatars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerAvatars.HarmonyPatches
{
    [HarmonyPatch(typeof(MenuInstaller), "InstallBindings", MethodType.Normal)]
    class LobbyAvatarPatch
    {
        static void Prefix(MenuInstaller __instance)
        {
            if (IPA.Loader.PluginManager.GetPluginFromId("CustomAvatar") != null)
            {
                MultiplayerLobbyAvatarController multiplayerLobbyAvatarController = __instance.GetField<MultiplayerLobbyAvatarController, MenuInstaller>("_multiplayerLobbyAvatarControllerPrefab");
                if (!multiplayerLobbyAvatarController.gameObject.TryGetComponent<CustomAvatarController>(out CustomAvatarController controller))
                {
                    multiplayerLobbyAvatarController.gameObject.AddComponent<CustomAvatarController>();
                }
            }
        }
    }
}
