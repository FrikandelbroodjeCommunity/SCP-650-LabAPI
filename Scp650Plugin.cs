using System;
using FrikanUtils.ServerSpecificSettings;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using Scp650Plugin.Debugging;

namespace Scp650Plugin
{
    public class Scp650Plugin : Plugin<Config>
    {
        public override string Name => "SCP-650";
        public override string Description => "";
        public override string Author => "Drakoor";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

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