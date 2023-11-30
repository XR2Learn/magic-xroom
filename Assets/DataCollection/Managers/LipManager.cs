using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataCollection.Managers
{ 
    public class LipManager : MonoBehaviour
    {
        [SerializeField]
        private ViveSR.anipal.Lip.SRanipal_Lip_Framework _lipFramework;

        private bool _enabled;

        private List<LipData> _dataBuffer;
        private System.Diagnostics.Stopwatch _stopwatch;

        private int _samplingRate;
        private int _samplesBufferSize;

        private bool _isStreaming;

        public void Awake()
        {
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.FaceTracking_Enabled, out _enabled, false);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.FaceTracking_SamplingRate, out _samplingRate, 10);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.FaceTracking_SamplingBufferSize, out _samplesBufferSize, 50);

            _lipFramework.EnableLip = _enabled;

            _stopwatch = new System.Diagnostics.Stopwatch();
            _dataBuffer = new List<LipData>();
        }

        public void FixedUpdate()
        {
            if (!_enabled || !_isStreaming) return;

            if (_stopwatch.Elapsed.TotalMilliseconds < (1000 / _samplingRate)) return;

            ViveSR.anipal.Lip.LipData lips = LipDataCollector._lipData;
            _dataBuffer.Add(new LipData(DateTime.Now.Ticks, lips));

            if (_dataBuffer.Count > _samplesBufferSize)
            {
                IOManager.Append(IOManager.Sensor.FACE, _dataBuffer);
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
