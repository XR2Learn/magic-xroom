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
using UnityEngine;
using Valve.VR;

namespace XR2Learn.DataCollection.Devices
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

        public override IData GetData()
        {
            return GetDevicePositionAndRotation();
        }

        private IData GetDevicePositionAndRotation()
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