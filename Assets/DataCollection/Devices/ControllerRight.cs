using Valve.VR;

namespace DataCollection.Devices
{
    public sealed class ControllerRight : AbstractVR
    {
        private static ControllerRight instance;

        private ControllerRight() : base("controller_right", ETrackedDeviceClass.Controller,
            ETrackedControllerRole.RightHand)
        {
        }

        public static ControllerRight Instance
        {
            get { return instance ??= new ControllerRight(); }
        }
    }
}