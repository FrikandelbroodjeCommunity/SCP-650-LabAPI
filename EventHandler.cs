using System.Linq;
using FrikanUtils.ProjectMer;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using Scp650Plugin.Debugging;
using UnityEngine;
using BreakableDoor = Interactables.Interobjects.BreakableDoor;
using Logger = LabApi.Features.Console.Logger;

namespace Scp650Plugin
{
    public static class EventHandler
    {
        private static Config Config => Scp650Plugin.Instance.Config;
        private static CoroutineHandle _debugRoutine;

        public static void RegisterEvents()
        {
            ServerEvents.RoundStarted += OnRoundStart;
            ServerEvents.RoundEnded += OnRoundEnd;
            PlayerEvents.Death += OnPlayerDead;

            if (!Scp650Plugin.Instance.Config.FollowTargetToPocketDimension)
            {
                PlayerEvents.EnteredPocketDimension += OnPocketed;
            }
        }

        public static void UnregisterEvents()
        {
            ServerEvents.RoundStarted -= OnRoundStart;
            ServerEvents.RoundEnded -= OnRoundEnd;
            PlayerEvents.Death -= OnPlayerDead;
            PlayerEvents.EnteredPocketDimension -= OnPocketed;
        }

        private static void OnRoundStart()
        {
            if (!Loader.DataLoaded)
            {
                Logger.Warn($"Not loaded yet... | {Loader.Poses != null} | {Loader.Scp650Schematic != null}");
                return;
            }

            for (var i = 0; i < Config.MaximumSpawnNumber; i++)
            {
                var possibleDoors = Door.List
                    .Where(x => Config.SpawnableZone.Contains(x.Zone) && x.Base is BreakableDoor).ToArray();
                if (possibleDoors.Length == 0)
                {
                    continue;
                }

                var door = possibleDoors.RandomItem();
                var transform = door.Transform;
                var pos = transform.position + 0.75f * (Random.Range(0, 1) * 2f - 1f) * transform.forward;
                var rotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
                var spawned = Loader.Scp650Schematic.SpawnSchematic(pos, rotation);
                spawned.gameObject.AddComponent<Scp650Ai>();

                Logger.Info("Spawned SCP-650 at " + door.DoorName + " | " + door.NameTag);
            }

            if (_debugRoutine.IsValid)
            {
                Timing.KillCoroutines(_debugRoutine);
            }

            _debugRoutine = Timing.RunCoroutine(DebugMenu.DebugRoutine());
        }

        private static void OnRoundEnd(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(_debugRoutine);
        }

        private static void OnPlayerDead(PlayerDeathEventArgs ev)
        {
            foreach (var ai in Scp650Ai.Instances.Where(ai => ai.Target == ev.Player))
            {
                ai.TargetKilled(ev.Attacker);
            }
        }

        private static void OnPocketed(PlayerEnteredPocketDimensionEventArgs ev)
        {
            foreach (var ai in Scp650Ai.Instances.Where(ai => ai.Target == ev.Player))
            {
                ai.Target = null;
            }
        }
    }
}