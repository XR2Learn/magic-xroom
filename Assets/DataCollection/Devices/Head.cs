using Valve.VR;

namespace DataCollection.Devices
{
    public sealed class Head : AbstractVR
    {
        private static Head instance;

        private Head() : base("head", ETrackedDeviceClass.HMD, ETrackedControllerRole.Invalid)
        {
        }

        public static Head Instance
        {
            get { return instance ??= new Head(); }
        }
    }
}