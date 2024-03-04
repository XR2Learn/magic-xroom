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
using Valve.VR;
using XR2Learn.DataCollection.Managers;

namespace XR2Learn.DataCollection.Devices
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
        public abstract IData GetData();

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