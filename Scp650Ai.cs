using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using ProjectMER.Features.Objects;
using SCP_650.Poses;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace SCP_650
{
    public class Scp650Ai : MonoBehaviour
    {
        public static readonly List<Scp650Ai> Instances = new List<Scp650Ai>();

        [NonSerialized] public Player Target;

        private const string Prefix = "mixamorig";

        private static Config Config => Scp650Plugin.Instance.Config;

        private bool _lookingForTarget = true;
        private float _targetFollowTimer;
        private float _followTime;
        private float _targetTeleportTimer;
        private float _teleportTime;
        private Posture _posture;
        private Player[] _seeingPlayers;

        private readonly Dictionary<string, GameObject> _joints = new Dictionary<string, GameObject> { };

        private void Start()
        {
            Instances.Add(this);
        }

        private void Update()
        {
            _seeingPlayers = Player.List
                .Where(player => !player.IsHost)
                .Where(player => IsWatching(transform.GetChild(0).transform.position, player))
                .Where(IsTargetable).ToArray();

            if (_seeingPlayers.Length != 0 && _lookingForTarget)
            {
                if (_lookingForTarget)
                {
                    Target = _seeingPlayers.RandomItem();

                    _lookingForTarget = false;
                    _followTime =
                        UnityEngine.Random.Range(Config.TargetFollowingMinTime, Config.TargetFollowingMaxTime);
                    _teleportTime = UnityEngine.Random.Range(Config.TeleportMinCoolTime, Config.TeleportMaxCoolTime);
                    _targetFollowTimer = 0f;
                    _targetTeleportTimer = 0f;
                }
            }

            if (Target == null) return;

            if (!Target.IsAlive)
            {
                Target = null;
                return;
            }

            _targetFollowTimer += Time.deltaTime;
            if (_targetFollowTimer >= _followTime)
            {
                _lookingForTarget = true;
                if (!Config.SmoothChangeTarget)
                {
                    return;
                }
            }

            _targetTeleportTimer += Time.deltaTime;
            if (_targetTeleportTimer >= _teleportTime && _seeingPlayers.Length == 0)
            {
                TryTeleport();
            }
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        public void ChildRegister(SchematicObject obj)
        {
            foreach (var block in obj.ObjectFromId.Values.Select(x => x.gameObject))
            {
                if (block.TryGetComponent(out PrimitiveObjectToy toy))
                {
                    toy.MovementSmoothing = 0;
                }

                if (block.name.Contains(Prefix))
                {
                    _joints.Add(block.name, block);
                }
            }

            ChangePose();
        }

        public void ChangePose()
        {
            // If there is only 1 pose, we cannot never select a new one.
            if (Loader.Poses.Postures.Count <= 1)
            {
                return;
            }

            Posture newPosture = null;
            while (newPosture == null)
            {
                newPosture = Loader.Poses.Postures.RandomItem();

                if (newPosture == _posture)
                {
                    newPosture = null;
                }
            }

            _posture = newPosture;
            foreach (var pair in _joints)
            {
                if (_posture.TransformPerJoint.TryGetValue(pair.Key.Replace($"{Prefix}:", ""), out var position))
                {
                    pair.Value.transform.localPosition = position[0];
                    pair.Value.transform.localRotation = Quaternion.Euler(position[1]);
                }
            }
        }

        public void TargetKilled(Player killer)
        {
            Target = null;

            if (killer == null)
            {
                return;
            }

            if (!Config.ChangeTargetToKillerWhenTargetKilled)
            {
                _lookingForTarget = true;
            }

            if (!IsTargetable(killer)) return;

            Target = killer;
            _lookingForTarget = false;
            _followTime = UnityEngine.Random.Range(Config.TargetFollowingMinTime, Config.TargetFollowingMaxTime);
            _teleportTime = UnityEngine.Random.Range(Config.TeleportMinCoolTime, Config.TeleportMaxCoolTime);
            _targetFollowTimer = 0f;
            _targetTeleportTimer = 0f;
        }

        public void TryTeleport()
        {
            // Check if there is 3 meters of room behind the player (we teleport 2 behind them)
            if (Physics.Raycast(Target.Camera.position, -Target.Camera.forward.NormalizeIgnoreY(), out _, 3f))
            {
                return;
            }

            // Get the ground position of the new teleport, height difference with the player can be at most 3 meters.
            if (!Physics.Raycast(Target.Camera.position - Target.Camera.forward.NormalizeIgnoreY() * 2f, Vector3.down,
                    out var hit, 3f))
            {
                return;
            }

            // Check if there are not any other instances within 2 meters of the teleportation target.
            if (Instances.Any(ai => ai != this && Vector3.Distance(ai.transform.position, hit.point) <= 2f))
            {
                return;
            }

            // Check if a non-target is looking at SCP-650
            if (Player.List.Where(player => !player.IsHost && player != Target)
                .Any(player => IsWatching(hit.point + new Vector3(0f, 1.2f, 0f), player)))
            {
                return;
            }

            var pos = hit.point;
            transform.position = pos;

            var normalized = (Target.Camera.position - pos).normalized;
            var b = Vector3.Angle(normalized, Vector3.forward) * Mathf.Sign(Vector3.Dot(normalized, Vector3.right));
            transform.rotation = Quaternion.Euler(0f, b, 0f);

            ChangePose();
            _teleportTime = UnityEngine.Random.Range(Config.TeleportMinCoolTime, Config.TeleportMaxCoolTime);
            _targetTeleportTimer = 0f;
        }

        private static bool IsWatching(Vector3 pos, Player player)
        {
            var roleBase = player.RoleBase;
            if (roleBase.RoleTypeId == RoleTypeId.Spectator ||
                !Config.ObserveEffectableFactions.Contains(roleBase.RoleTypeId.GetFaction()) ||
                Config.ObserveEffectableBlacklistRoles.Contains(roleBase.RoleTypeId))
            {
                return false;
            }

            var vector = (pos - player.Camera.position).normalized;
            var vector1 = player.Camera.forward;
            if (Vector3.Dot(vector, vector1) >= 0.1f &&
                VisionInformation.GetVisionInformation(player.ReferenceHub, player.Camera, pos, 0.8f, 60f).IsLooking)
            {
                return true;
            }

            return Vector3.Distance(pos, player.Camera.position) <= 1.2f;
        }

        private bool IsTargetable(Player player)
        {
            if (!Config.OverlapTarget && Instances.Any(ai => ai != this && ai.Target == player))
            {
                return false;
            }

            return player != Target && Config.TargetableFactions.Contains(player.Faction) &&
                   !Config.TargetBlacklistRoles.Contains(player.Role);
        }
    }
}