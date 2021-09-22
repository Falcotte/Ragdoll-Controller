using UnityEngine;
using UnityEditor;

namespace AngryKoala.Ragdoll
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RagdollCreator))]
    public class RagdollCreatorEditor : Editor
    {
        [MenuItem("GameObject/Ragdoll/Add Ragdoll", false, 12)]
        private static void AddRagdoll()
        {
            foreach(var selected in Selection.gameObjects)
            {
                RagdollCreator ragdollCreator = selected.AddComponent<RagdollCreator>();

                ragdollCreator.CreateRagdoll();
                DestroyImmediate(ragdollCreator);
            }
        }

        [MenuItem("GameObject/Ragdoll/Add Ragdoll", true)]
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

        [MenuItem("GameObject/Ragdoll/Remove Ragdoll", false, 12)]
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

        [MenuItem("GameObject/Ragdoll/Remove Ragdoll", true)]
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

            if(skinnedMeshRenderer.rootBone.name == "mixamorig:Hips")
            {
                foreach(var part in Rig.MixamoPartNames)
                {
                    if(!transform.FindRecursive(part))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if(skinnedMeshRenderer.rootBone.name == "root.x")
            {
                foreach(var part in Rig.AutoRigPartNames)
                {
                    if(!transform.FindRecursive(part))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
