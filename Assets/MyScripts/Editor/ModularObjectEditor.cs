using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModularObject), true)]
public class ModularObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ModularObject script = (ModularObject)target;

       

        if (GUILayout.Button("Refresh Modules"))
        {
            if (script != null)
            {
                script.RegisterAllModulesFromEditor();
            } else
            {
                Debug.Log("this script is null broski");
            }
            
        }
    }
}