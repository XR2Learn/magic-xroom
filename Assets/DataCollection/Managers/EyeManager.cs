using DataCollection.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataCollection.Managers
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

            _eyeFramework.EnableEye = _enabled;

            _stopwatch = new System.Diagnostics.Stopwatch();
            _dataBuffer = new List<EyeData>();
        }

        public void FixedUpdate()
        {
            if (!_enabled || !_isStreaming) return;

            if (_stopwatch.Elapsed.TotalMilliseconds < (1000 / _samplingRate)) return;

            ViveSR.anipal.Eye.EyeData eyes = EyeDataCollector.eyeData;
            _dataBuffer.Add(new EyeData(DateTime.Now.Ticks, eyes));

            if (_dataBuffer.Count > _samplesBufferSize)
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