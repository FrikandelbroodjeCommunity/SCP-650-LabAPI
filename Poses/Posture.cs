using System.Collections.Generic;
using UnityEngine;

namespace SCP_650.Poses
{
    public class Posture
    {
        public string PoseName { get; set; } = "Standing";

        public Dictionary<string, Vector3[]> TransformPerJoint { get; set; }
    }
}