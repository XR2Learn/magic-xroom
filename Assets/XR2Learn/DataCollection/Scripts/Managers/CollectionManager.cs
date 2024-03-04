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


using System.Collections.Generic;
using XR2Learn.DataCollection.Devices;
using UnityEngine;

namespace XR2Learn.DataCollection.Managers
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