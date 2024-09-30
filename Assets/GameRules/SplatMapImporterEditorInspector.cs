using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplatMapImporterEditor))]
public class SplatMapImporterEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector UI

        SplatMapImporterEditor script = (SplatMapImporterEditor)target;

        // Add a button to the Inspector
        if (GUILayout.Button("Apply Splat Map"))
        {
            script.ApplySplatMapInEditor();
        }
    }
}
