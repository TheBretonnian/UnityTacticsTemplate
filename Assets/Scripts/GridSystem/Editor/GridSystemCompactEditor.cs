using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSystemCompact))]
public class GridSystemCompactEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystemCompact gridSystem = (GridSystemCompact)target;
        if (GUILayout.Button("Generate Grid"))
        {
            gridSystem.CreateGrid();
        }
        if (GUILayout.Button("Delete Grid"))
        {
            gridSystem.DeleteGrid();
        }
    }
}
