using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using LabApi.Features.Wrappers;
using Scp650Plugin.Debugging;
using Scp650Plugin.Poses;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace Scp650Plugin
{
    public class Scp650Ai : MonoBehaviour
    {
        public static readonly List<Scp650Ai> Instances = new List<Scp650Ai>();
        
        [NonSerialized] public Player Target;

        private static Config Config => Scp650Plugin.Instance.Config;

        private bool _lookingForTarget = true;
        private float _targetFollowTimer;
        private float _followTime;
        private float _targetTeleportTimer;
        private float _teleportTime;
        private Posture _posture;
        private Player[] _seeingPlayers;

        private readonly Dictionary<string, GameObject> _joints = new Dictionary<string, GameObject> { };
        private readonly Dictionary<string, GameObject> _jointHelper = new Dictionary<string, GameObject> { };
        
        private void Start()
        {
            Instances.Add(this);

            ChildRegister(gameObject);
            ChangePose();
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
                if (_posture.TransformPerJoint.TryGetValue(pair.Key.Replace("mixamorig:", ""), out var position))
                {
                    pair.Value.transform.localPosition = _jointHelper.TryGetValue(pair.Key, out var @object)
                        ? @object.transform.localPosition
                        : position[0];

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

        public void TryTeleport(List<Player> blinker = null)
        {
            if (Physics.Raycast(Target.Camera.position, -Target.Camera.forward.NormalizeIgnoreY(), out _, 3f))
            {
                return;
            }

            if (!Physics.Raycast(Target.Camera.position - Target.Camera.forward.NormalizeIgnoreY() * 2f, Vector3.down,
                    out var hit, 3f))
            {
                return;
            }

            if (Instances.Any(ai => ai != this && Vector3.Distance(ai.transform.position, hit.point) <= 2f))
            {
                return;
            }

            foreach (var player in Player.List.Where(player => !player.IsHost && player != Target))
            {
                if (blinker != null)
                {
                    if (blinker.Contains(player))
                    {
                        continue;
                    }
                }

                if (IsWatching(hit.point + new Vector3(0f, 1.2f, 0f), player))
                {
                    return;
                }
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

        private void ChildRegister(GameObject @object)
        {
            for (var i = 0; i < @object.transform.childCount; i++)
            {
                var game = @object.transform.GetChild(i).gameObject;
                if (game.TryGetComponent(out PrimitiveObjectToy toy))
                {
                    toy.MovementSmoothing = 0;
                }

                if (game.name.Contains("mixamorig"))
                {
                    if (toy != null)
                    {
                        _jointHelper.Add(game.name, game);
                    }
                    else
                    {
                        _joints.Add(game.name, game);
                        ChildRegister(game);
                    }
                }
            }
        }
    }
}