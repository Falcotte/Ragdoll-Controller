using UnityEngine;
using UnityEditor;

namespace AngryKoala.Ragdoll
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RagdollCreator))]
    public class RagdollCreatorEditor : Editor
    {
        [MenuItem("GameObject/Angry Koala/Ragdoll/Add Ragdoll", false, 12)]
        private static void AddRagdoll()
        {
            foreach(var selected in Selection.gameObjects)
            {
                RagdollCreator ragdollCreator = selected.AddComponent<RagdollCreator>();

                ragdollCreator.CreateRagdoll();
                DestroyImmediate(ragdollCreator);
            }
        }

        [MenuItem("GameObject/Angry Koala/Ragdoll/Add Ragdoll", true)]
        private static bool AddRagdollValidation()
        {
            if(Selection.gameObjects.Length == 0)
            {
                return false;
            }

            foreach(var selected in Selection.gameObjects)
            {
                if(selected == null || !CheckRig(selected.transform) || selected.GetComponentInChildren<Ragdoll>())
                {
                    return false;
                }
            }
            return true;
        }

        [MenuItem("GameObject/Angry Koala/Ragdoll/Remove Ragdoll", false, 12)]
        private static void RemoveRagdoll()
        {
            foreach(var selected in Selection.gameObjects)
            {
                Ragdoll ragdoll = selected.GetComponentInChildren<Ragdoll>();

                foreach(var transform in ragdoll.GetComponentsInChildren<Transform>())
                {
                    if(transform != ragdoll.transform)
                    {
                        DestroyImmediate(transform.GetComponent<RagdollComponent>());

                        DestroyImmediate(transform.GetComponent<CharacterJoint>());

                        DestroyImmediate(transform.GetComponent<Rigidbody>());
                        DestroyImmediate(transform.GetComponent<Collider>());
                    }
                }

                DestroyImmediate(ragdoll);
            }
        }

        [MenuItem("GameObject/Angry Koala/Ragdoll/Remove Ragdoll", true)]
        private static bool RemoveRagdollValidation()
        {
            if(Selection.gameObjects.Length == 0)
            {
                return false;
            }

            foreach(var selected in Selection.gameObjects)
            {
                if(selected == null || !selected.GetComponentInChildren<Ragdoll>())
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CheckRig(Transform transform)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
            if(!skinnedMeshRenderer)
            {
                return false;
            }

            int rigIndex = -1;

            for(int i = 0; i < Rig.RigTypes.Length; i++)
            {
                if(skinnedMeshRenderer.rootBone.name == Rig.RigTypes[i].Pelvis)
                {
                    rigIndex = i;
                    break;
                }
            }

            if(rigIndex == -1) return false;

            foreach(var partName in Rig.RigTypes[rigIndex].PartNames)
            {
                if(!transform.FindRecursive(partName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
