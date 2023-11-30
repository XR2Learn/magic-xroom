using DataCollection.Managers;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(VRManager))]
    public class DataCollectionMainManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VRManager t = (VRManager)serializedObject.targetObject;

            DrawDefaultInspector();
            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Data collection status", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is Running", t.IsEnabled);
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || t.IsEnabled);
            if(GUILayout.Button("Start collection"))
                t.StartCollection();
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || !t.IsEnabled);
            if(GUILayout.Button("Stop collection"))
                t.StopCollection();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }
    }
}