using HarmonyLib;
using IPA;
using MultiplayerAvatars.Installers;
using SiraUtil.Zenject;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerAvatars
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static readonly string HarmonyId = "com.github.Goobwabber.MultiplayerAvatars";
        public static readonly string UserAgent = $"MultiplayerAvatars/{Assembly.GetExecutingAssembly().GetName().Version} {VersionInfo.Description}";
        internal static Plugin Instance { get; private set; } = null!;
        internal static IPALogger Log { get; private set; } = null!;
        internal static Harmony Harmony = null!;
        internal static Zenjector Zenjector = null!;

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Harmony = new Harmony(HarmonyId);
            Zenjector = zenjector;
            Zenjector.OnApp<AvatarInstaller>();
            Plugin.Log?.Debug("Init finished.");
        }

        [OnEnable]
        public void OnEnable()
        {
            Plugin.Log?.Info(UserAgent);
            Harmony.PatchAll();
        }

        [OnDisable]
        public void OnDisable()
        {
            Harmony.UnpatchAll(HarmonyId);
        }

    }
}
