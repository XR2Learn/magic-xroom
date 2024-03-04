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
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace XR2Learn.Common
{
    public class LongPressButton : HoverButton
    {
        [Header("Long press parameters")]
        [SerializeField]
        [Min(0.01f)]
        private float pressDelay;
        [SerializeField][Min(0.01f)] private float decayMultiplier;

        public UnityEvent<float> onLongPressProgress = new UnityEvent<float>();
        public UnityEvent onLongPressComplete = new UnityEvent();

        private float buttonDownTime;
        private float buttonUpTime;
        private bool doProgressUpdate;

        private float progress;
        private float progressAtBtnDown;

        private const float Tolerance = .01f;

        public float Progress => progress;

        private void Awake()
        {
            onButtonDown.AddListener(OnButtonDown);
            onButtonUp.AddListener(OnButtonUp);
        }

        private void OnDestroy()
        {
            onButtonDown.RemoveListener(OnButtonDown);
            onButtonUp.RemoveListener(OnButtonUp);
        }

        private void Update()
        {
            if (!doProgressUpdate) return;


            float pressTime = Time.time - buttonDownTime;
            float depressTime = Time.time - buttonUpTime;
            if (engaged)
            {
                float delay = (1 - progressAtBtnDown) * pressDelay;
                progress = Mathf.Clamp01(progressAtBtnDown + pressTime / delay);
            }
            else
            {
                progress = Mathf.Clamp01(progress - Time.deltaTime * decayMultiplier / pressDelay);
            }

            onLongPressProgress?.Invoke(progress);


            switch (engaged)
            {
                case true when pressTime > (1 - progressAtBtnDown) * pressDelay:
                    onLongPressComplete?.Invoke();
                    doProgressUpdate = false;
                    break;
                case false when depressTime > decayMultiplier / pressDelay:
                    doProgressUpdate = false;
                    break;
            }
        }

        private void OnButtonDown(Hand _)
        {
            buttonDownTime = Time.time;
            progressAtBtnDown = progress;
            doProgressUpdate = true;
        }

        private void OnButtonUp(Hand _)
        {
            buttonUpTime = Time.time;
            doProgressUpdate = true;
        }
    }
}