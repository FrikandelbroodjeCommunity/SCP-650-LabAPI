using System.ComponentModel;
using MapGeneration;
using PlayerRoles;

namespace SCP_650
{
    public class Config
    {
        [Description("The folder the schematic is in")]
        public string SchematicFolder { get; set; } = "Maps";

        [Description("The name of the schematic file")]
        public string SchematicName { get; set; } = "SCP650.json";

        [Description("The folder the poses file is in")]
        public string PoseFolder { get; set; } = "Poses";

        [Description("The name of the file that contains the poses")]
        public string PoseFile { get; set; } = "global.yml";

        [Description("The maximum amount of SCP-650 instances that can be spawned")]
        public int MaximumSpawnNumber { get; set; } = 1;

        [Description("The zones SCP-650 can spawn in")]
        public FacilityZone[] SpawnableZone { get; set; } =
        {
            FacilityZone.LightContainment,
            FacilityZone.HeavyContainment,
            FacilityZone.Entrance
        };

        [Description("The factions that influence SCP-650 when looking at it")]
        public Faction[] ObserveEffectableFactions { get; set; } =
        {
            Faction.FoundationEnemy,
            Faction.FoundationStaff
        };

        [Description("Roles that are blacklisted from influencing SCP-650 when looking at it")]
        public RoleTypeId[] ObserveEffectableBlacklistRoles { get; set; } = { };

        [Description("Factions that SCP-650 can chase")]
        public Faction[] TargetableFactions { get; set; } =
        {
            Faction.FoundationEnemy,
            Faction.FoundationStaff
        };

        [Description("Roles that are blacklisted from being chased by SCP-650")]
        public RoleTypeId[] TargetBlacklistRoles { get; set; } = { };

        [Description("When enabled, multiple SCP-650 instances can chase the same player")]
        public bool OverlapTarget { get; set; } = false;

        [Description("The minimum time in seconds it follows its target")]
        public float TargetFollowingMinTime { get; set; } = 50f;

        [Description("The maximum time in seconds it follows its target")]
        public float TargetFollowingMaxTime { get; set; } = 120f;

        [Description("If following time passes, " +
                     "select whether SCP-650 will follow original target until next targetable player appears. " +
                     "Or it will just freeze at that point and wait.")]
        public bool SmoothChangeTarget { get; set; } = true;

        [Description("When enabled, if the target for SCP-650 gets killed, the killer becomes the new target")]
        public bool ChangeTargetToKillerWhenTargetKilled { get; set; } = true;

        [Description("The minimum amount of time between teleports")]
        public float TeleportMinCoolTime { get; set; } = 5f;

        [Description("The maximum amount of time between teleports")]
        public float TeleportMaxCoolTime { get; set; } = 10f;

        [Description("Whether SCP-650 will follow the player into the pocket dimension")]
        public bool FollowTargetToPocketDimension { get; set; } = true;
    }
}