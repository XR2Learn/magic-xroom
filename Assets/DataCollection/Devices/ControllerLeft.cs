using Valve.VR;

namespace DataCollection.Devices
{
    public sealed class ControllerLeft : AbstractVR
    {
        private static ControllerLeft instance;

        private ControllerLeft() : base("controller_left", ETrackedDeviceClass.Controller,
            ETrackedControllerRole.LeftHand)
        {
        }

        public static ControllerLeft Instance
        {
            get { return instance ??= new ControllerLeft(); }
        }
    }
}