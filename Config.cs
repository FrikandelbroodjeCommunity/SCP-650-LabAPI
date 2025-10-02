using MapGeneration;
using PlayerRoles;
using System.ComponentModel;

namespace Scp650Plugin
{
    public class Config
    {
        //Setting
        public string SchematicFolder { get; set; } = "Maps";
        public string SchematicName { get; set; } = "SCP650.json";
        public string PoseFolder { get; set; } = "Poses";
        public string PoseFile { get; set; } = "global.yml";

        //Spawning
        public int MaximumSpawnNumber { get; set; } = 1;
        public FacilityZone[] SpawnableZone { get; set; } = {
            FacilityZone.LightContainment,
            FacilityZone.HeavyContainment,
            FacilityZone.Entrance
        };

        //Targeting
        public Faction[] ObserveEffectableFactions { get; set; } = {
            Faction.FoundationEnemy,
            Faction.FoundationStaff
        };
        
        public RoleTypeId[] ObserveEffectableBlacklistRoles { get; set; } = { };
        
        public Faction[] TargetableFactions { get; set; } = {
            Faction.FoundationEnemy,
            Faction.FoundationStaff
        };
        
        public RoleTypeId[] TargetBlacklistRoles { get; set; } = { };
        
        public bool OverlapTarget { get; set; } = false;
        public int TargetingAmbient { get; set; } = -1;


        //Target
        public float TargetFollowingMinTime { get; set; } = 50f;
        public float TargetFollowingMaxTime { get; set; } = 120f;
        [Description("If following time passes, select whether SCP-650 will follow original target until next targetable player appears. Or it will just freeze at that point and wait.")]
        public bool SmoothChangeTarget { get; set; } = true;
        public bool ChangeTargetToKillerWhenTargetKilled { get; set; } = true;
        public float TeleportMinCoolTime { get; set; } = 5f;
        public float TeleportMaxCoolTime { get; set; } = 10f;
        public bool FollowTargetToPocketDimension { get; set; } = true;
    }
}
