using HarmonyLib;
using IPA;
using IPA.Loader;
using MultiplayerAvatars.HarmonyPatches;
using MultiplayerAvatars.Installers;
using MultiplayerAvatars.Utilities;
using SiraUtil.Zenject;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerAvatars
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static readonly string HarmonyId = "com.github.Goobwabber.MultiplayerAvatars";
        
        internal static Plugin Instance { get; private set; } = null!;
        internal static PluginMetadata PluginMetadata = null!;
        internal static IPALogger Log { get; private set; } = null!;

        internal static HttpClient HttpClient { get; private set; } = null!;
        internal static Harmony? _harmony = null!;
        internal static Harmony Harmony
		{
            get
			{
                return _harmony ??= new Harmony(HarmonyId);
			}
		}

        public static string UserAgent
		{
            get
			{
                var modVersion = PluginMetadata.Version.ToString();
                var bsVersion = IPA.Utilities.UnityGame.GameVersion.ToString();
                return $"MultiplayerAvatars/{modVersion} (BeatSaber/{bsVersion})";
			}
		}

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector, PluginMetadata pluginMetadata)
        {
            Instance = this;
            PluginMetadata = pluginMetadata;
            Log = logger;
            
            zenjector.OnApp<AvatarInstaller>();
            zenjector.OnGame<GameAvatarInstaller>().OnlyForMultiplayer();

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", Plugin.UserAgent);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Plugin.Log?.Info($"MultiplayerAvatars: '{VersionInfo.Description}'");

            HarmonyManager.ApplyDefaultPatches();
            Task versionTask = CheckVersion();

            Harmony.PatchAll();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            
        }

        public async Task CheckVersion()
        {
            try
            {
                GithubVersion latest = await VersionCheck.GetLatestVersionAsync("Goobwabber", "MultiplayerAvatars");
                Log?.Debug($"Latest version is {latest}, released on {latest.ReleaseDate.ToShortDateString()}");
                if (PluginMetadata != null)
                {
                    SemVer.Version currentVer = PluginMetadata.Version;
                    SemVer.Version latestVersion = new SemVer.Version(latest.ToString());
                    bool updateAvailable = new SemVer.Range($">{currentVer}").IsSatisfied(latestVersion);
                    if (updateAvailable)
                    {
                        Log?.Info($"An update is available!\nNew mod version: {latestVersion}\nCurrent mod version: {currentVer}");
                    }
                }
            }
            catch (ReleaseNotFoundException ex)
            {
                Log?.Warn(ex.Message);
            }
            catch (Exception ex)
            {
                Log?.Warn($"Error checking latest version: {ex.Message}");
                Log?.Debug(ex);
            }
        }
    }
}
