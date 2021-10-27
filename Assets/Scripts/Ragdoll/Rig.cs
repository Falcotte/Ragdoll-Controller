using System.Linq;
using UnityEngine;

namespace AngryKoala.Ragdoll
{
    public class Rig
    {
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

        public static RigType[] RigTypes
        {
            get
            {
                return Resources.LoadAll("RigTypes", typeof(RigType)).Cast<RigType>().ToArray();
            }
        }

        private int rigIndex = -1;

        public void DetermineRigType(Transform transform)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();

            if(skinnedMeshRenderer == null)
            {
                Debug.LogWarning($"No skinned mesh renderer found in {transform.name}");
                return;
            }

            for(int i = 0; i < RigTypes.Length; i++)
            {
                if(skinnedMeshRenderer.rootBone.name == RigTypes[i].Pelvis)
                {
                    rigIndex = i;
                    break;
                }
            }

            if(rigIndex == -1)
            {
                Debug.LogWarning($"No suitable rig types found in Resources/RigTypes");
                return;
            }
        }

        public void GetParts(Transform transform)
        {
            Pelvis = transform.FindRecursive(RigTypes[rigIndex].Pelvis);
            LeftHips = transform.FindRecursive(RigTypes[rigIndex].LeftHips);
            LeftKnee = transform.FindRecursive(RigTypes[rigIndex].LeftKnee);
            RightHips = transform.FindRecursive(RigTypes[rigIndex].RightHips);
            RightKnee = transform.FindRecursive(RigTypes[rigIndex].RightKnee);
            LeftArm = transform.FindRecursive(RigTypes[rigIndex].LeftArm);
            LeftElbow = transform.FindRecursive(RigTypes[rigIndex].LeftElbow);
            RightArm = transform.FindRecursive(RigTypes[rigIndex].RightArm);
            RightElbow = transform.FindRecursive(RigTypes[rigIndex].RightElbow);
            MiddleSpine = transform.FindRecursive(RigTypes[rigIndex].MiddleSpine);
            Head = transform.FindRecursive(RigTypes[rigIndex].Head);
        }
    }
}
