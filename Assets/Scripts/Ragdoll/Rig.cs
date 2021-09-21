using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class Rig
    {
        public static List<string> MixamoRigPartNames = new List<string> { "mixamorig:Hips", "mixamorig:LeftUpLeg", "mixamorig:LeftLeg",
            "mixamorig:RightUpLeg","mixamorig:RightLeg","mixamorig:LeftArm","mixamorig:LeftForeArm","mixamorig:RightArm","mixamorig:RightForeArm",
            "mixamorig:Spine1","mixamorig:Head" };

        public Transform Pelvis { get; private set; }

        public Transform LeftHips { get; private set; }
        public Transform LeftKnee { get; private set; }

        public Transform RightHips { get; private set; }
        public Transform RightKnee { get; private set; }

        public Transform LeftArm { get; private set; }
        public Transform LeftElbow { get; private set; }

        public Transform RightArm { get; private set; }
        public Transform RightElbow { get; private set; }

        public Transform MiddleSpine { get; private set; }
        public Transform Head { get; private set; }

        private enum RigTypes { Mixamo }
        private RigTypes rigtype;

        public void DetermineRigType(Transform transform)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

            if(skinnedMeshRenderer == null)
            {
                Debug.LogWarning($"No skinned mesh renderer found in {transform.name}");
                return;
            }

            if(transform.GetComponentInChildren<SkinnedMeshRenderer>().rootBone.name == "mixamorig:Hips")
            {
                rigtype = RigTypes.Mixamo;
            }
        }

        public void GetParts(Transform transform)
        {
            switch(rigtype)
            {
                case RigTypes.Mixamo:
                    GetMixamoParts(transform);
                    break;
            }
        }

        private void GetMixamoParts(Transform transform)
        {
            Pelvis = transform.FindRecursive("mixamorig:Hips");
            LeftHips = transform.FindRecursive("mixamorig:LeftUpLeg");
            LeftKnee = transform.FindRecursive("mixamorig:LeftLeg");
            RightHips = transform.FindRecursive("mixamorig:RightUpLeg");
            RightKnee = transform.FindRecursive("mixamorig:RightLeg");
            LeftArm = transform.FindRecursive("mixamorig:LeftArm");
            LeftElbow = transform.FindRecursive("mixamorig:LeftForeArm");
            RightArm = transform.FindRecursive("mixamorig:RightArm");
            RightElbow = transform.FindRecursive("mixamorig:RightForeArm");
            MiddleSpine = transform.FindRecursive("mixamorig:Spine1");
            Head = transform.FindRecursive("mixamorig:Head");
        }
    }
}
