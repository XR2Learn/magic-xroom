using System.Collections.Generic;
using DataCollection.Devices;
using UnityEngine;

namespace DataCollection.Managers
{
    public class CollectionManager
    {
        private static CollectionManager _instance;

        private readonly HashSet<IDeviceable> _devices;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        private CollectionManager()
        {
            _devices = new HashSet<IDeviceable>();
            _stopwatch = new System.Diagnostics.Stopwatch();
        }

        public static CollectionManager Instance
        {
            get { return _instance ??= new CollectionManager(); }
        }

        public bool RegisterDevice(IDeviceable device)
        {
            if (device == null)
            {
                Debug.LogWarning("No valid device");
                return false;
            }

            if (!device.IsPresent())
            {
                Debug.LogWarning("Device not present");
                return false;
            }

            bool success = _devices.Add(device);
            if (!success) Debug.LogWarning("Device " + device.GetName() + " already added");
            return success;
        }

        public bool RemoveDevice(IDeviceable device)
        {
            if (device != null) return _devices.Remove(device);
            Debug.LogError("No valid device");
            return false;
        }

        public void StartCollection()
        {
            _stopwatch.Start();
            Debug.Log("Collection started");
        }

        public void StopCollection()
        {
            _stopwatch.Stop();
            Debug.Log("Collection stopped");
        }
    }
}