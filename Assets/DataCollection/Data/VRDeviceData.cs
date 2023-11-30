using UnityEngine;

namespace DataCollection.Data
{
    public class VRDeviceData : IDatable
    {
        private readonly bool isEmpty;

        private readonly Vector3 _position;
        private readonly Quaternion _rotation;

        public VRDeviceData(Vector3 position, Quaternion rotation)
        {   
            _position = position;
            _rotation = rotation;
            isEmpty = false;
        }

        public VRDeviceData()
        {
            _position = Vector3.zero;
            _rotation = Quaternion.identity;
            isEmpty = true;
        }

        public bool IsEmpty()
        {
            return isEmpty;
        }

        public override string ToString()
        {
            return _position.x + ","
                 + _position.y + ","
                 + _position.z + ","
                 + _rotation.x + ","
                 + _rotation.y + ","
                 + _rotation.z + ","
                 + _rotation.w;
        }
    }
}