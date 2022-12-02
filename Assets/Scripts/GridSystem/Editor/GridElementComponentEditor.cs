using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridElementComponent))]
public class GridElementComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridElementComponent gridElementHelper = (GridElementComponent)target;
        if (GUILayout.Button("Toggle Obstacle"))
        {
            gridElementHelper.ToggleObstacle();
        }

    }
}
