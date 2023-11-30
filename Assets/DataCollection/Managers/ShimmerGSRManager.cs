
using System;
using System.Collections.Generic;
using UnityEngine;
using XR2Learn_ShimmerAPI;

namespace DataCollection.Managers
{ 
    public partial class ShimmerGSRManager : MonoBehaviour
    {
        public enum ConnectionState 
        {
            CONNECTED, CONNECTING, DISCONNECTED, STREAMING, INACTIVE
        }

        #region Serialized fields

        [Serializable]
        private struct ShimmerDevice
        {
            [Tooltip("Enable/disable device")]
            [SerializeField] public bool Enabled;
            [Tooltip("User defined device name")]
            [SerializeField] public string DeviceName;
        }
        [SerializeField] private ShimmerDevice _device = new();

        [Serializable]
        private struct ShimmerDeviceConfig 
        {
            [Tooltip("Number of Heart Beats required to calculate an average")]
            [SerializeField] public int HeartBeatsToAverage;
            [Tooltip("PPG algorithm training period in [s]")]
            [SerializeField] public int TrainingPeriodPPG;
            [Tooltip("PPG low-pass filter cutoff in [Hz]")]
            [SerializeField] public double LowPassFilterCutoff;
            [Tooltip("PPG high-pass filter cutoff in [Hz]")]
            [SerializeField] public double HighPassFilterCutoff;
            [Range(0.01f, 1000.0f)]
            [Tooltip("Shimmer device internal sampling rate in [Hz]")]
            [SerializeField] public double SamplingRate;
            [Range(1, 100)]
            [Tooltip("Number of samples to buffer before writing to file")]
            [SerializeField] public int SamplesBufferSize;
        }
        [SerializeField] private ShimmerDeviceConfig _config = new();

        [Serializable]
        private struct ShimmerDeviceSensors
        {
            [Tooltip("Flag to enable/disable the Accelerator sensor")]
            [SerializeField] public bool EnableAccelerator;
            [Tooltip("Flag to enable/disable the GSR sensor")]
            [SerializeField] public bool EnableGSR;
            [Tooltip("Flag to enable/disable the PPG sensor")]
            [SerializeField] public bool EnablePPG;
        }
        [SerializeField] private ShimmerDeviceSensors _sensors = new();

        #endregion

        private XR2Learn_ShimmerGSR Shimmer;
        private System.Diagnostics.Stopwatch _stopwatch;
        private List<XR2Learn_ShimmerGSRData> _dataBuffer;
        public ConnectionState State { get; private set; }

        private static ShimmerGSRManager _instance;
        public static ShimmerGSRManager Instance
        {
            get { return _instance ??= FindObjectOfType<ShimmerGSRManager>(); }
        }


        private ShimmerGSRManager() { }

        private void Reset()
        {
            _device.Enabled = true;
            _device.DeviceName = "Shimmer3";

            _config.HeartBeatsToAverage = XR2Learn_ShimmerGSR.DefaultNumberOfHeartBeatsToAverage;
            _config.TrainingPeriodPPG = XR2Learn_ShimmerGSR.DefaultTrainingPeriodPPG;
            _config.LowPassFilterCutoff = XR2Learn_ShimmerGSR.DefaultLowPassFilterCutoff;
            _config.HighPassFilterCutoff = XR2Learn_ShimmerGSR.DefaultHighPassFilterCutoff;
            _config.SamplingRate = XR2Learn_ShimmerGSR.DefaultSamplingRate;
            _config.SamplesBufferSize = 50;

            _sensors.EnableAccelerator = XR2Learn_ShimmerGSR.DefaultEnableAccelerator;
            _sensors.EnableGSR = XR2Learn_ShimmerGSR.DefaultEnableGSR;
            _sensors.EnablePPG = XR2Learn_ShimmerGSR.DefaultEnablePPG;
        }

        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_Enabled, out _device.Enabled, true);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_DeviceName, out _device.DeviceName, "Shimmer3");

            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_HeartbeatsToAverage, out _config.HeartBeatsToAverage, XR2Learn_ShimmerGSR.DefaultNumberOfHeartBeatsToAverage);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_TrainingPeriodPPG, out _config.TrainingPeriodPPG, XR2Learn_ShimmerGSR.DefaultTrainingPeriodPPG);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_SamplingRate, out _config.SamplingRate, XR2Learn_ShimmerGSR.DefaultSamplingRate);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_SamplingBufferSize, out _config.SamplesBufferSize, 50);

            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_EnableAccelerator, out _sensors.EnableAccelerator, XR2Learn_ShimmerGSR.DefaultEnableAccelerator);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_EnableGSR, out _sensors.EnableGSR, XR2Learn_ShimmerGSR.DefaultEnableGSR);
            UserSettingsLoader.Load(UserSettingsLoader.userSettings.Shimmer_EnablePPG, out _sensors.EnablePPG, XR2Learn_ShimmerGSR.DefaultEnablePPG);

            Shimmer = new XR2Learn_ShimmerGSR();
            State = _device.Enabled ? ConnectionState.DISCONNECTED : ConnectionState.INACTIVE;

            _dataBuffer = new List<XR2Learn_ShimmerGSRData>();
            _stopwatch = new System.Diagnostics.Stopwatch();

            if (!_device.Enabled) return;

            new System.Threading.Thread(AttemptConnection).Start();
        }

        private void FixedUpdate()
        {
            if (!_device.Enabled) return;

            if (!Shimmer.IsConnected()) return;

            if (State == ConnectionState.STREAMING && _stopwatch.ElapsedMilliseconds < (1000 / Shimmer.SamplingRate))
            { 
                HandleData();
                _stopwatch.Restart();
            }
        }

        public void Configure(string serialPort)
        {
            Shimmer.Configure(_device.DeviceName, serialPort);

            Shimmer.NumberOfHeartBeatsToAverage = _config.HeartBeatsToAverage;
            Shimmer.TrainingPeriodPPG = _config.TrainingPeriodPPG;
            Shimmer.LowPassFilterCutoff = _config.LowPassFilterCutoff;
            Shimmer.HighPassFilterCutoff = _config.HighPassFilterCutoff;
            Shimmer.SamplingRate = _config.SamplingRate;
            Shimmer.EnableAccelerator = _sensors.EnableAccelerator;
            Shimmer.EnableGSR = _sensors.EnableGSR;
            Shimmer.EnablePPG = _sensors.EnablePPG;
        }

        private void HandleData()
        {
            if (Shimmer.LatestData != null) _dataBuffer.Add(Shimmer.LatestData);

            if (_dataBuffer.Count > _config.SamplesBufferSize) 
            {
                IOManager.Append(IOManager.Sensor.SHIMMER, _dataBuffer);
                _dataBuffer.Clear();
            }
        }

        public void OnApplicationQuit()
        {
            if (!_device.Enabled) return;

            StopStreaming();
            Disconnect();
        }

        public void AttemptConnection()
        {
            State = ConnectionState.CONNECTING;

            string[] serialPorts = XR2Learn_SerialPortsManager.GetAvailableSerialPortsNames();
            LogUtils.LogShimmer("Avaialbe serial ports: [ " + string.Join(", ", serialPorts) + " ]");
            foreach (string serialPort in serialPorts)
            {
                Configure(serialPort);

                LogUtils.LogShimmer("Attempting connection on serial port " + serialPort + " ...");
                Shimmer.Connect();
                if (Shimmer.IsConnected())
                {
                    LogUtils.LogShimmer("Shimmer connected on port " + serialPort);
                    State = ConnectionState.CONNECTED;
                    return;
                }
            }
            LogUtils.LogShimmer("Unable to connect to Shimmer device");
            State = ConnectionState.DISCONNECTED;
        }

        private void Disconnect()
        {
            LogUtils.LogShimmer("Disconnecting from Shimmer device");
            Shimmer.Disconnect();
            State = ConnectionState.DISCONNECTED;
        }

        public void StartStreaming()
        {
            LogUtils.LogShimmer("Shimmer device Start streaming");
            Shimmer.StartStreaming();
            State = ConnectionState.STREAMING;

            _stopwatch.Start();
        }

        public void StopStreaming()
        {
            LogUtils.LogShimmer("Shimmer device Stop streaming");
            Shimmer.StopStreaming();
            State = ConnectionState.CONNECTED;

            _stopwatch.Reset();
        }

        public bool IsEnabled()
        {
            return _device.Enabled;
        }
    }
}
