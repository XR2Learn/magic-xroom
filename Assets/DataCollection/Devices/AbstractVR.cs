using DataCollection.Data;
using UnityEngine;
using Valve.VR;

namespace DataCollection.Devices
{
    public abstract class AbstractVR : AbstractDevice
    {
        private readonly ETrackedDeviceClass deviceClass;
        private readonly ETrackedControllerRole controllerRole;

        protected AbstractVR(string name, ETrackedDeviceClass deviceClass,
            ETrackedControllerRole controllerRole) : base(name)
        {
            this.deviceClass = deviceClass;
            this.controllerRole = controllerRole;
        }

        public override bool IsPresent()
        {
            return OpenVR.IsHmdPresent();
        }

        public override bool IsAvailable()
        {
            for (uint deviceIndex = 0; deviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++deviceIndex)
            {
                if (OvrSystem.GetTrackedDeviceClass(deviceIndex) == deviceClass)
                {
                    return true;
                }
            }
            return false;
        }

        public override IDatable GetData()
        {
            return GetDevicePositionAndRotation();
        }

        private IDatable GetDevicePositionAndRotation()
        {
            for (uint deviceIndex = 0; deviceIndex < OpenVR.k_unMaxTrackedDeviceCount; ++deviceIndex)
            {
                if (OvrSystem.GetTrackedDeviceClass(deviceIndex) != deviceClass)
                {
                    continue;
                }

                if (controllerRole != ETrackedControllerRole.Invalid &&
                    OvrSystem.GetControllerRoleForTrackedDeviceIndex(deviceIndex) != controllerRole)
                {
                    continue;
                }

                TrackedDevicePose_t pose = TrackedDevicePoseArray[deviceIndex];

                if (!pose.bDeviceIsConnected || !pose.bPoseIsValid)
                {
                    return new VRDeviceData();
                }

                HmdMatrix34_t deviceData = pose.mDeviceToAbsoluteTracking;
                Vector3 position = deviceData.GetPosition();
                Quaternion rotation = deviceData.GetRotation();
                return new VRDeviceData(position, rotation);
            }

            return new VRDeviceData();
        }
    }
}