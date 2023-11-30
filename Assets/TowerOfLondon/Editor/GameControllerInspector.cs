using TowerOfLondon.Scripts;
using UnityEditor;
using UnityEngine;

namespace TowerOfLondon.Editor {
    [CustomEditor(typeof(TowerOfHanoiGameController))]
    public class GameControllerInspector : UnityEditor.Editor {
        private Level _lvl;

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            TowerOfHanoiGameController tol = (TowerOfHanoiGameController)serializedObject.targetObject;

            if (GUILayout.Button("Save as Level")) {
                Level level = CreateInstance<Level>();

                level.redStart = tol.Discs.Red.transform.localPosition;
                level.redTarget = tol.Discs.Red.Target.transform.localPosition;
                level.greenStart = tol.Discs.Green.transform.localPosition;
                level.greenTarget = tol.Discs.Green.Target.transform.localPosition;
                level.blueStart = tol.Discs.Blue.transform.localPosition;
                level.blueTarget = tol.Discs.Blue.Target.transform.localPosition;

                level.maxMoves = tol.MaxMoves;

                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/TowerOfLondon/Levels/level.asset");
                if (!AssetDatabase.IsValidFolder("Assets/TowerOfLondon/Levels"))
                    AssetDatabase.CreateFolder("Assets/TowerOfLondon", "Levels");
                AssetDatabase.CreateAsset(level, path);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _lvl = (Level)EditorGUILayout.ObjectField(_lvl, typeof(Level), false);
            if (GUILayout.Button("Load")) {
                tol.LoadLevel(_lvl);
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}