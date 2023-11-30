using DataCollection.Data;
using System;
using UnityEngine;
using XR2Learn_ShimmerAPI;

namespace DataCollection.Managers
{ 
    public class MainDataCollectionManager : MonoBehaviour
    {
        public VRManager VRManager;
        public ShimmerGSRManager ShimmerManager;
        public EyeManager EyeManager;
        public LipManager LipManager;

        [NonSerialized]
        public bool IsRunning = false;

        public void Start()
        {
            if (UserSettingsLoader.loaded && UserSettingsLoader.userSettings.DataCollection_AutoStart.Loaded && UserSettingsLoader.userSettings.DataCollection_AutoStart.Value)
                StartDataCollection();
        }

        [ContextMenu("Toggle Data Collection")]
        public void ToggleDataCollection()
        {
            if (IsRunning) StopDataCollection();
            else StartDataCollection();
        }

        [ContextMenu("Start Data Collection")]
        private void StartDataCollection()
        {
            IOManager.Init();
            IOManager.AppendHeader(IOManager.Sensor.PROGRESS_EVENT, UserEvents.HEADER);
            IOManager.AppendHeader(IOManager.Sensor.VR, VRData.HEADER);
            if (ShimmerManager != null && ShimmerManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.SHIMMER, XR2Learn_ShimmerGSRData.HEADER);
            if (EyeManager != null && EyeManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.EYE, EyeData.HEADER);
            if (LipManager != null && LipManager.IsEnabled())
                IOManager.AppendHeader(IOManager.Sensor.FACE, LipData.HEADER);

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
        private void StopDataCollection()
        {
            VRManager?.StopCollection();
            if (ShimmerManager.IsEnabled())
                ShimmerManager?.StopStreaming();
            if (EyeManager.IsEnabled())
                EyeManager?.StopCollection();
            if (LipManager.IsEnabled())
                LipManager?.StopCollection();

            IsRunning = false;
        }
    }
}
