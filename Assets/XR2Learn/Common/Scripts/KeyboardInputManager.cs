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
using UnityEngine.InputSystem;
using XR2Learn.Common.Feedback;
using XR2Learn.Common.UserConfig;
using XR2Learn.DataCollection.Managers;

namespace XR2Learn.Common
{
    public class KeyboardInputManager : MonoBehaviour
    {
        [SerializeField]
        private bool _enabled;
        [SerializeField]
        private MainDataCollectionManager _mainDataCollectionManager;

        public void Awake()
        {
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Keyboard_EnableShortcuts, out _enabled, false);
            gameObject.SetActive(_enabled);
        }

        public void Update()
        {
            if (Keyboard.current.fKey.wasReleasedThisFrame)
            {
                FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.MANUAL);
            }
            if (Keyboard.current.dKey.wasReleasedThisFrame)
            {
                _mainDataCollectionManager?.ToggleDataCollection();
            }
        }
    }
}
