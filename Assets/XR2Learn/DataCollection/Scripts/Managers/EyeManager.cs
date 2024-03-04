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
using System.Collections.Generic;
using UnityEngine;
using XR2Learn.Common.UserConfig;

namespace XR2Learn.DataCollection.Managers
{ 
    public class EyeManager : MonoBehaviour
    {
        [SerializeField]
        private ViveSR.anipal.Eye.SRanipal_Eye_Framework _eyeFramework;

        private bool _enabled;

        private List<EyeData> _dataBuffer;
        private System.Diagnostics.Stopwatch _stopwatch;

        private int _samplingRate;
        private int _samplesBufferSize;

        private bool _isStreaming;

        public void Awake()
        {
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.EyeTracking_Enabled, out _enabled, false);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.EyeTracking_SamplingRate, out _samplingRate, 10);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.EyeTracking_SamplingBufferSize, out _samplesBufferSize, 50);

            _eyeFramework.gameObject.SetActive(_enabled);
            _eyeFramework.EnableEye = _enabled;

            gameObject.SetActive(_enabled);

            _stopwatch = new System.Diagnostics.Stopwatch();
            _dataBuffer = new List<EyeData>();
        }

        public void FixedUpdate()
        {
            if (!_enabled || !_isStreaming) return;

            if (_stopwatch.Elapsed.TotalMilliseconds < (1000 / _samplingRate)) return;

            ViveSR.anipal.Eye.EyeData eyes = EyeDataCollector.eyeData;
            _dataBuffer.Add(new EyeData(DateTime.Now.Ticks, eyes));

            if (_dataBuffer.Count >= _samplesBufferSize)
            {
                IOManager.Append(IOManager.Sensor.EYE, _dataBuffer);
                _dataBuffer.Clear();
            }
            _stopwatch.Restart();
        }

        public void StartCollection()
        {
            _isStreaming = true;
            _stopwatch.Start();
        }

        public void StopCollection()
        {
            _isStreaming = false;
            _stopwatch.Reset();
        }

        public bool IsEnabled()
        {
            return _enabled;
        }
    }
}