using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor : Editor
{
    private float transparency = 1.0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridSystem gridSystem = (GridSystem)target;

        // Update Transparency on Editor
        if (GUI.changed)
        {
            if(transparency != gridSystem.Transparency)
            {
                transparency = gridSystem.Transparency;
                gridSystem.UpdateTransparency();
            }
        }

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
