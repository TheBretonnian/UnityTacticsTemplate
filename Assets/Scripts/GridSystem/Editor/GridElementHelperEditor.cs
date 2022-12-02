using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridElementHelper))]
public class GridElementHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridElementHelper gridElementHelper = (GridElementHelper)target;
        if (GUILayout.Button("Toggle Obstacle"))
        {
            gridElementHelper.ToggleObstacle();
        }

    }
}
