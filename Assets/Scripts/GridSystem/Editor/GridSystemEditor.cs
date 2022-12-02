using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystem gridSystem = (GridSystem)target;
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
