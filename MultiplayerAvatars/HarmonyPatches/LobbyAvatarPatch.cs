using HarmonyLib;
using MultiplayerAvatars.Avatars;

namespace MultiplayerAvatars.HarmonyPatches
{
    [HarmonyPatch(typeof(MenuInstaller), "InstallBindings", MethodType.Normal)]
    internal class LobbyAvatarPatch
    {
        internal static void Prefix(ref MultiplayerLobbyAvatarController ____multiplayerLobbyAvatarControllerPrefab)
        {
            if (!____multiplayerLobbyAvatarControllerPrefab.gameObject.TryGetComponent<CustomAvatarController>(out CustomAvatarController _))
            {
                ____multiplayerLobbyAvatarControllerPrefab.gameObject.AddComponent<CustomAvatarController>();
            }
        }
    }
}
