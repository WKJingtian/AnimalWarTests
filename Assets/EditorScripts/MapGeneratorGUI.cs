using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenerationController))]
public class MapGeneratorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerationController m = (MapGenerationController)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("try new map"))
            m.OnGenerateBtnPressed();
    }
}
