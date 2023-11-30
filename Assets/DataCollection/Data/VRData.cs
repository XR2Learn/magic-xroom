
namespace DataCollection.Data
{
    public class VRData
    {
        public const string HEADER = "timestamp,head_posX,head_posY,head_posZ,head_rotX,head_rotY,head_rotZ,head_rotW," +
        "lcontroller_posX,lcontroller_pposY,lcontroller_posZ,lcontroller_rotX,lcontroller_rotY,lcontroller_rotZ,lcontroller_rotW," +
        "rcontroller_posX,rcontroller_pposY,rcontroller_posZ,rcontroller_rotX,rcontroller_rotY,rcontroller_rotZ,rcontroller_rotW";

        private readonly long _timestamp;
        private readonly VRDeviceData _head;
        private readonly VRDeviceData _leftController;
        private readonly VRDeviceData _rightController;

        public VRData(long timestamp, VRDeviceData head, VRDeviceData leftController, VRDeviceData rightController)
        {
            _timestamp = timestamp;
            _head = head;
            _leftController = leftController;
            _rightController = rightController;
        }

        public override string ToString()
        {
            return _timestamp + "," + _head + "." + _leftController + "," + _rightController;
        }
    }
}