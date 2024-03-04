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


using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace XR2Learn.Common
{
    [RequireComponent(typeof(SteamVR_LoadLevel))]
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private List<SceneReference> scenes;

        private SteamVR_LoadLevel _loadLevel;
        private bool _isLoading;

        private void Awake()
        {
            _loadLevel = GetComponent<SteamVR_LoadLevel>();
        }

        public void QuitToDesktop()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public void SetLocale(Locale locale)
        {
            LocalizationSettings.Instance.SetSelectedLocale(locale);
        }

        public void LoadSceneAsync(SceneReference scene)
        {
            LoadSceneAsync(scene.BuildIndex);
        }

        public void LoadSceneAsync(int index)
        {
            if (index >= scenes.Count || index < 0) return;
            SceneReference s = scenes[index];
            if (!s.IsSafeToUse) return;
            _loadLevel.levelName = scenes[index].Name;
            _isLoading = true;
            _loadLevel.Trigger();
        }

        private void Update()
        {
            if (_isLoading) return;
            const int key = (int)KeyCode.Keypad0;
            for (int i = 0; i < scenes.Count; i++)
            {
                if (SceneManager.GetActiveScene().buildIndex == i) continue;
                if (Input.GetKeyUp((KeyCode)key + i))
                    LoadSceneAsync(scenes[i]);
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}