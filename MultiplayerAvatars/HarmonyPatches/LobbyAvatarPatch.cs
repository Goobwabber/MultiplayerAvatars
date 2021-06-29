using HarmonyLib;
using MultiplayerAvatars.Avatars;
using System;

namespace MultiplayerAvatars.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLobbyInstaller), "InstallBindings", MethodType.Normal)]
    internal class LobbyAvatarPatch
    {
        internal static void Prefix(ref MultiplayerLobbyAvatarController ____multiplayerLobbyAvatarControllerPrefab)
        {
            try
            {
                if (!____multiplayerLobbyAvatarControllerPrefab.gameObject.TryGetComponent<CustomAvatarController>(out CustomAvatarController _))
                {
                    ____multiplayerLobbyAvatarControllerPrefab.gameObject.AddComponent<CustomAvatarController>();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
                throw;
            }
        }
    }
}
