/*********** Copyright © 2024 University of Applied Sciences of Southern Switzerland (SUPSI) ***********\
 
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial
 portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*******************************************************************************************************/


using UnityEngine;
using XR2Learn.Scenarios.TowerOfLondon;

namespace UnityEditor
{
    [CustomEditor(typeof(TowerOfHanoiController))]
    public class GameControllerInspector : Editor
    {
        private Level _lvl;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            TowerOfHanoiController tol = (TowerOfHanoiController)serializedObject.targetObject;

            if (GUILayout.Button("Save as Level"))
            {
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
            if (GUILayout.Button("Load"))
            {
                tol.LoadLevel(_lvl);
            }

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}