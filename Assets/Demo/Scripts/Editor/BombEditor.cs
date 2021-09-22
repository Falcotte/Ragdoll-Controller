using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bomb))]
public class BombEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Bomb bomb = (Bomb)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Explode"))
        {
            bomb.Explode();
        }
    }
}
