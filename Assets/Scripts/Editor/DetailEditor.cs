using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DetailEditor : EditorWindow
{    
    public static void ShowWindow()
    {
        GetWindow<DetailEditor>("DetailEditor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Other Editor Content", EditorStyles.boldLabel);
    }
}