using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RagdollDemo))]
public class RagdollDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RagdollDemo ragdollDemo = (RagdollDemo)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Enable Ragdoll"))
        {
            ragdollDemo.EnableRagdoll();
        }

        if(GUILayout.Button("Disable Ragdoll"))
        {
            ragdollDemo.DisableRagdoll();
        }
    }
}
