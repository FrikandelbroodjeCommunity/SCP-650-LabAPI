using System;
using FrikanUtils.ServerSpecificSettings;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using SCP_650.Debugging;

namespace SCP_650
{
    public class Scp650Plugin : Plugin<Config>
    {
        public const string VersionString = "1.0.1.0";
        
        public override string Name => "SCP-650";
        public override string Description => "";
        public override string Author => "Drakoor";
        public override Version Version => new Version(VersionString);
        public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

        public static Scp650Plugin Instance { get; private set; }

        public override void Enable()
        {
            Instance = this;
            Loader.LoadData();
            EventHandler.RegisterEvents();
            SSSHandler.RegisterMenu(DebugMenu.Instance);
        }

        public override void Disable()
        {
            EventHandler.UnregisterEvents();
            SSSHandler.UnregisterMenu(DebugMenu.Instance);
        }
    }
}