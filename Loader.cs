using FrikanUtils.FileSystem;
using FrikanUtils.FileSystem.Providers;
using ProjectMER.Features.Serializable.Schematics;

namespace SCP_650
{
    public static class Loader
    {
        private const string DefaultSchematicFileName = "frikan.SCP650.json";
        private const string DefaultPoseFileName = "frikan.SCP650.yml";
        
        public static Poses.Poses Poses { get; private set; }
        public static SchematicObjectDataList Scp650Schematic { get; private set; }

        public static bool DataLoaded => Poses != null && Scp650Schematic != null;

        private static Config Config => Scp650Plugin.Instance.Config;


        public static async void LoadData()
        {
            var url = BackupFileProvider.GetGithubUrl("FrikandelbroodjeCommunity", "SCP-650-LabAPI", DefaultPoseFileName);
            BackupFileProvider.RegisterBackup(Scp650Plugin.Instance, Config.PoseFile, Config.PoseFolder, url);
            
            url = BackupFileProvider.GetGithubUrl("FrikandelbroodjeCommunity", "SCP-650-LabAPI", DefaultSchematicFileName);
            BackupFileProvider.RegisterBackup(Scp650Plugin.Instance, Config.SchematicName, Config.SchematicFolder, url);
            
            Poses = await FileHandler.SearchFile<Poses.Poses>(Config.PoseFile, Config.PoseFolder, false);
            Scp650Schematic = await FileHandler.SearchFile<SchematicObjectDataList>(Config.SchematicName,
                Config.SchematicFolder, true);
            Scp650Schematic.Path = "";
        }
    }
}