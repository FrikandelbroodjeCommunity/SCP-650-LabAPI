using System.Collections.Generic;
using UnityEngine;

namespace Scp650Plugin.Poses
{
    public class Posture
    {
        public string PoseName { get; set; } = "Standing";

        public Dictionary<string, Vector3[]> TransformPerJoint { get; set; }
    }
}