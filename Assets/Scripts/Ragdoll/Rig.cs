using System.Collections.Generic;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class Rig
    {
        public static List<string> MixamoPartNames = new List<string> { "mixamorig:Hips", "mixamorig:LeftUpLeg", "mixamorig:LeftLeg",
            "mixamorig:RightUpLeg", "mixamorig:RightLeg", "mixamorig:LeftArm", "mixamorig:LeftForeArm", "mixamorig:RightArm", "mixamorig:RightForeArm",
            "mixamorig:Spine1", "mixamorig:Head" };

        public static List<string> AutoRigPartNames = new List<string> { "root.x", "thigh_stretch.l", "leg_stretch.l", "thigh_stretch.r",
            "leg_stretch.r", "arm_stretch.l", "forearm_stretch.l", "arm_stretch.r", "forearm_stretch.r", "spine_02.x", "head.x" };

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

        // More rig types can be added here
        private enum RigTypes { Mixamo, AutoRig }
        private RigTypes rigtype;

        public void DetermineRigType(Transform transform)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

            if(skinnedMeshRenderer == null)
            {
                Debug.LogWarning($"No skinned mesh renderer found in {transform.name}");
                return;
            }

            if(skinnedMeshRenderer.rootBone.name == "mixamorig:Hips")
            {
                rigtype = RigTypes.Mixamo;
            }
            else if(skinnedMeshRenderer.rootBone.name == "root.x")
            {
                rigtype = RigTypes.AutoRig;
            }
        }

        public void GetParts(Transform transform)
        {
            switch(rigtype)
            {
                case RigTypes.Mixamo:
                    GetMixamoParts(transform);
                    break;
                case RigTypes.AutoRig:
                    GetAutoRigParts(transform);
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

        private void GetAutoRigParts(Transform transform)
        {
            Pelvis = transform.FindRecursive("root.x");
            LeftHips = transform.FindRecursive("thigh_stretch.l");
            LeftKnee = transform.FindRecursive("leg_stretch.l");
            RightHips = transform.FindRecursive("thigh_stretch.r");
            RightKnee = transform.FindRecursive("leg_stretch.r");
            LeftArm = transform.FindRecursive("arm_stretch.l");
            LeftElbow = transform.FindRecursive("forearm_stretch.l");
            RightArm = transform.FindRecursive("arm_stretch.r");
            RightElbow = transform.FindRecursive("forearm_stretch.r");
            MiddleSpine = transform.FindRecursive("spine_02.x");
            Head = transform.FindRecursive("head.x");
        }
    }
}
