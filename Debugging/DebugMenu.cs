using System.Collections.Generic;
using System.Text;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using MEC;

namespace SCP_650.Debugging
{
    public class DebugMenu : MenuBase
    {
        public override string Name => "SCP-650 Debug Menu";
        public override MenuType Type => MenuType.Dynamic;

        public static readonly DebugMenu Instance = new DebugMenu();

        private const ushort TextArea = 0;

        public override bool HasPermission(Player player)
        {
            return base.HasPermission(player) && player.HasPermissions("frikanutils.debug");
        }

        public override IEnumerable<IServerSpecificSetting> GetSettings(Player player)
        {
            yield return new TextArea(TextArea, GetText());
        }

        public void UpdateAll()
        {
            var text = GetText();
            foreach (var area in GetAllFields<TextArea>(TextArea))
            {
                area.Label = text;
            }
        }

        public static IEnumerator<float> DebugRoutine()
        {
            while (true)
            {
                Instance.UpdateAll();
                yield return Timing.WaitForSeconds(10);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static string GetText()
        {
            if (Scp650Ai.Instances.Count == 0)
            {
                return "There are currently no SCP-650 instances.";
            }

            var builder = new StringBuilder($"There are currently {Scp650Ai.Instances.Count} instances:\n\n");
            foreach (var instance in Scp650Ai.Instances)
            {
                builder.Append(" - ");
                builder.AppendLine(Room.GetRoomAtPosition(instance.transform.position)?.ToString() ??
                                   "Currently not in a room");
            }

            return builder.ToString();
        }
    }
}