using DataCollection.Data;
using DataCollection.Managers;
using Valve.VR;

namespace DataCollection.Devices
{
    public abstract class AbstractDevice : IDeviceable
    {
        private readonly string name;
        private const ETrackingUniverseOrigin Origin = ETrackingUniverseOrigin.TrackingUniverseStanding;

        protected static readonly TrackedDevicePose_t[] TrackedDevicePoseArray =
            new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

        protected static readonly CVRSystem OvrSystem = OpenVR.System;
        private const int PredictedSecondsToPhotonsFromNow = 0;

        protected AbstractDevice(string name)
        {
            this.name = name;
            VRManager.Instance.RefreshingData += DataCollectionMainManager_RefreshingData;
        }

        public abstract bool IsPresent();
        public abstract bool IsAvailable();
        public abstract IDatable GetData();

        public string GetName()
        {
            return name;
        }

        private static void DataCollectionMainManager_RefreshingData()
        {
            OvrSystem.GetDeviceToAbsoluteTrackingPose(Origin, PredictedSecondsToPhotonsFromNow, TrackedDevicePoseArray);
        }
    }
}