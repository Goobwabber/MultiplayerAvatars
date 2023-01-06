using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Loader;
using MultiplayerAvatars.Installers;
using SiraUtil.Zenject;
using System;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerAvatars
{
    [Plugin(RuntimeOptions.DynamicInit)]
    class Plugin
    {
        public const string ID = "com.goobwabber.MultiplayerAvatars";

        internal static IPALogger Logger = null!;

        private readonly Harmony _harmony;
        private readonly PluginMetadata _metadata;

        [Init]
        public Plugin(IPALogger logger, PluginMetadata pluginMetadata, Zenjector zenjector)
        {
            _harmony = new Harmony(ID);
            _metadata = pluginMetadata;
            Logger = logger;

            zenjector.UseMetadataBinder<Plugin>();
            zenjector.UseLogger(logger);
            zenjector.UseHttpService();
            zenjector.UseSiraSync(SiraUtil.Web.SiraSync.SiraSyncServiceType.GitHub, "Goobwabber", "MultiplayerAvatars");
            zenjector.Install<MpavAppInstaller>(Location.App);
            zenjector.Install<MpavLobbyInstaller, MultiplayerLobbyInstaller>();
            zenjector.Install<MpavGameInstaller>(Location.MultiplayerCore);
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(_metadata.Assembly);
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }
    }
}
