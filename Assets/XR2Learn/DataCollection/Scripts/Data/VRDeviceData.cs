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


using System.Text;
using UnityEngine;

namespace XR2Learn.DataCollection.Data
{
    public class VRDeviceData : IData
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
            string comma = ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(_position.x);
            sb.Append(comma);
            sb.Append(_position.y);
            sb.Append(comma);
            sb.Append(_position.z);
            sb.Append(comma);
            sb.Append(_rotation.x);
            sb.Append(comma);
            sb.Append(_rotation.y);
            sb.Append(comma);
            sb.Append(_rotation.z);
            sb.Append(comma);
            sb.Append(_rotation.w);

            return sb.ToString();
        }
    }
}