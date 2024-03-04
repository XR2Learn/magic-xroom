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


//using UnityEngine;
//using UnityEngine.Events;
using Valve.VR;

namespace XR2Learn.Common
{
    public class HapticsManager //: MonoBehaviour
    {
        /*
        [SerializeField]
        public UnityEvent OnLeftHandTriggerDown;
        [SerializeField]
        public UnityEvent OnRightHandTriggerDown;
        [SerializeField]
        public UnityEvent OnLeftHandUITriggerDown;
        [SerializeField]
        public UnityEvent OnRightHandUITriggerDown;
        */

        public enum Controller
        {
            LEFT, RIGHT, BOTH
        }

        /*
        public void Start()
        {
            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch").onStateDown += (action, sources) =>
            {
                if (sources.Equals(SteamVR_Input_Sources.LeftHand))
                    OnLeftHandTriggerDown.Invoke();
                if (sources.Equals(SteamVR_Input_Sources.RightHand))
                    OnRightHandTriggerDown.Invoke();
            };

            SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI").onStateDown += (action, sources) =>
            {
                if (sources.Equals(SteamVR_Input_Sources.LeftHand))
                    OnLeftHandUITriggerDown.Invoke();
                if (sources.Equals(SteamVR_Input_Sources.RightHand))
                    OnRightHandUITriggerDown.Invoke();
            };
        }
        */

        public static void Vibrate(Controller hand, float delay, float duration, float frequency, float intensity)
        {
            if (hand != Controller.LEFT)
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic").Execute(delay, duration, frequency, intensity, SteamVR_Input_Sources.RightHand);
            if (hand != Controller.RIGHT)
                SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic").Execute(delay, duration, frequency, intensity, SteamVR_Input_Sources.LeftHand);
        }

        public static void VibrateShort(Controller hand)
        {
            Vibrate(hand, 0.0f, 0.1f, 120.0f, 0.8f);
        }

        public static void VibrateLong(Controller hand)
        {
            Vibrate(hand, 0.0f, 0.3f, 120.0f, 0.8f);
        }
    }
}
