using FrikanUtils.FileSystem;
using ProjectMER.Features.Serializable.Schematics;

namespace Scp650Plugin
{
    public static class Loader
    {
        public static Poses.Poses Poses { get; private set; }
        public static SchematicObjectDataList Scp650Schematic { get; private set; }

        public static bool DataLoaded => Poses != null && Scp650Schematic != null;

        private static Config Config => Scp650Plugin.Instance.Config;


        public static async void LoadData()
        {
            Poses = await FileHandler.SearchFile<Poses.Poses>(Config.PoseFile, Config.PoseFolder, false);
            Scp650Schematic = await FileHandler.SearchFile<SchematicObjectDataList>(Config.SchematicName,
                Config.SchematicFolder, true);
            Scp650Schematic.Path = "";
        }
    }
}