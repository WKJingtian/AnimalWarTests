using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Map))]
public class MapGeneratorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        Map m = (Map)target;
        //base.OnInspectorGUI();
        if (DrawDefaultInspector())
            m.GenerateNew();
        else if (GUILayout.Button("try new map"))
            m.GenerateNew();
    }
}
