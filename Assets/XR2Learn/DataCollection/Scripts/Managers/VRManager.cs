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


using System;
using System.Collections.Generic;
using XR2Learn.DataCollection.Data;
using XR2Learn.DataCollection.Devices;
using UnityEngine;
using Valve.VR;
using XR2Learn.Common.UserConfig;

namespace XR2Learn.DataCollection.Managers
{
    public sealed class VRManager : MonoBehaviour
    {
        private static VRManager _instance;

        private System.Diagnostics.Stopwatch _stopwatch;
        private List<VRData> _dataBuffer;
        public event Action RefreshingData;

        [SerializeField] private bool _isEnabled;
        [SerializeField] private int _samplingRate;
        [SerializeField] private int _samplesBufferSize;

        private bool _isStreaming;

        public static VRManager Instance
        {
            get { return _instance ??= FindObjectOfType<VRManager>(); }
        }

        public bool IsEnabled => _isEnabled;

        private void Awake()
        {
            if (!_isEnabled) return;

            if (_instance == null)
            {
                _instance = this;
            }

            if (!InitOpenVR())
            {
                _isEnabled = false;
                return;
            }
            _isStreaming = false;

            UserSettingsLoader.Load(UserSettingsLoader.userSettings.VR_SamplingRate, out _samplingRate, 10);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.VR_SamplingBufferSize, out _samplesBufferSize, 50);

            _stopwatch = new System.Diagnostics.Stopwatch();
            _dataBuffer = new List<VRData>();
            RegisterDevices();
        }

        private void FixedUpdate()
        {
            if (!_isStreaming)
            {
                return;
            }

            if (_stopwatch.Elapsed.TotalMilliseconds > (1000 / _samplingRate))
            {
                if (_stopwatch.Elapsed.TotalMilliseconds > 1000)
                {
                    Debug.LogError("!!! VR data not updating !!! [" + _stopwatch.Elapsed.TotalMilliseconds + "]");
                }
                OnUpdate();
                _stopwatch.Restart();
            }
        }

        private void OnDestroy()
        {
            OpenVR.Shutdown();
            StopCollection();
        }

        private static void RegisterDevices()
        {
            CollectionManager collectionManager = CollectionManager.Instance;
            collectionManager.RegisterDevice(Head.Instance);
            collectionManager.RegisterDevice(ControllerLeft.Instance);
            collectionManager.RegisterDevice(ControllerRight.Instance);
        }

        private void OnUpdate(bool bypass = false)
        {
            RefreshingData?.Invoke();
            
            _dataBuffer.Add(new VRData(
                DateTime.Now.Ticks,
                (VRDeviceData)Head.Instance.GetData(),
                (VRDeviceData)ControllerLeft.Instance.GetData(),
                (VRDeviceData)ControllerRight.Instance.GetData()
            ));

            if (bypass || _dataBuffer.Count >= _samplesBufferSize)
            {
                IOManager.Append(IOManager.Sensor.VR, _dataBuffer);
                _dataBuffer.Clear();
            }
        }

        public void StartCollection()
        {
            if (_isStreaming)
            {
                Debug.LogWarning("Cannot start, collection already in progress.");
                return;
            }
            Debug.LogWarning("Starting VR data collection.");

            _isStreaming = true;
            _stopwatch.Start();
            CollectionManager.Instance.StartCollection();
        }

        public void StopCollection()
        {
            if (!_isStreaming)
            {
                Debug.LogWarning("Cannot stop, no collection in progress");
                return;
            }

            CollectionManager.Instance.StopCollection();
            _isStreaming = false;
            _stopwatch.Reset();
            _dataBuffer.Clear();
        }

        public int GetSamplingRate()
        {
            return _samplingRate;
        }

        private static bool InitOpenVR()
        {
            if (OpenVR.System != null)
            {
                Debug.Log("OpenVR context correctly initialized");
                return true;
            }

            Debug.Log("Trying to initialize OpenVR");
            EVRInitError error = EVRInitError.None;
            OpenVR.Init(ref error);

            if (error == EVRInitError.None)
            {
                Debug.Log("OpenVR context correctly initialized");
                return true;
            }
            Debug.LogError("Failed to initialize OpenVR: " + error);
            return false;
        }
    }
}