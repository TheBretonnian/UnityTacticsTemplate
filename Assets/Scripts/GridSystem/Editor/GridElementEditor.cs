using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridElement))]
public class GridElementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridElement gridElement = (GridElement)target;
        if (GUILayout.Button("Toggle Obstacle"))
        {
            gridElement.ToggleObstacle();
        }

    }
}
