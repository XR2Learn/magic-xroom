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


using XR2Learn.DataCollection.Data;
using System;
using UnityEngine;
using UnityEngine.Events;
using XR2Learn.Common;

namespace XR2Learn.DataCollection.Managers
{ 
    public class MainDataCollectionManager : MonoBehaviour
    {
        public VRManager VRManager;
        public ShimmerGSRManager ShimmerManager;
        public EyeManager EyeManager;
        public LipManager LipManager;

        [SerializeField] private UnityEvent _OnDataCollectionStopped;

        [NonSerialized]
        public bool IsRunning = false;

        [ContextMenu("Toggle Data Collection")]
        public void ToggleDataCollection()
        {
            if (IsRunning)
            {
                StopDataCollection();
            }
            else
            {
                StartDataCollection();
            }
        }

        [ContextMenu("Start Data Collection")]
        public void StartDataCollection()
        {
            SoundManager.PlaySound("DataCollectionToggle");
            HapticsManager.Vibrate(HapticsManager.Controller.BOTH, 0, 0.1f, 120f, 0.8f);

            IOManager.Init();
            IOManager.AppendHeader(IOManager.Sensor.PROGRESS_EVENT, IData.Headers.EVENTS);
            IOManager.AppendHeader(IOManager.Sensor.VR, IData.Headers.VR);

            if (ShimmerManager != null && ShimmerManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.SHIMMER, IData.Headers.SHIMMER);
            if (EyeManager != null && EyeManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.EYE, IData.Headers.EYE);
            if (LipManager != null && LipManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.FACE, IData.Headers.LIP);

            VRManager?.StartCollection();
            if (ShimmerManager.IsEnabled())
                ShimmerManager?.StartStreaming();
            if (EyeManager.IsEnabled())
                EyeManager?.StartCollection();
            if (LipManager.IsEnabled())
                LipManager?.StartCollection();

            IsRunning = true;
        }

        [ContextMenu("Stop Data Collection")]
        public void StopDataCollection()
        {
            SoundManager.PlaySound("DataCollectionToggle", true);
            HapticsManager.Vibrate(HapticsManager.Controller.BOTH, 0, 0.1f, 120f, 0.8f);

            VRManager?.StopCollection();
            if (ShimmerManager.IsEnabled())
                ShimmerManager?.StopStreaming();
            if (EyeManager.IsEnabled())
                EyeManager?.StopCollection();
            if (LipManager.IsEnabled())
                LipManager?.StopCollection();

            IsRunning = false;

            _OnDataCollectionStopped.Invoke();
        }
    }
}
