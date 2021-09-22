using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AngryKoala.Ragdoll
{
    [CustomEditor(typeof(Ragdoll))]
    public class RagdollEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Ragdoll ragdoll = (Ragdoll)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Select Ragdoll Components"))
            {
                List<Object> objects = new List<Object>();
                foreach(var ragdollComponent in ragdoll.RagdollComponents)
                {
                    objects.Add(ragdollComponent.gameObject);
                }

                Selection.objects = objects.ToArray();
            }
        }
    }
}