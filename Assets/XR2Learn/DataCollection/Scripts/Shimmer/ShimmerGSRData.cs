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


using ShimmerAPI;
using System.Text;

namespace XR2Learn.DataCollection.Shimmer
{
    /// <summary>
    /// Class representing a Shimmer device dataframe
    /// </summary>
    public class ShimmerGSRData
    {
        #region Instance variables

        /// <summary>
        /// Timestamp 
        /// </summary>
        public readonly long Timestamp;

        /// <summary>
        /// Internal timestamp 
        /// </summary>
        public readonly SensorData IntTimestamp;
        /// <summary>
        /// Accelerator X axis Data and Unit
        /// </summary>
        public readonly SensorData AcceleratorX;
        /// <summary>
        /// Accelerator Y axis Data and Unit
        /// </summary>
        public readonly SensorData AcceleratorY;
        /// <summary>
        /// Accelerator Z axis Data and Unit
        /// </summary>
        public readonly SensorData AcceleratorZ;
        /// <summary>
        /// Galvanic Skip Response (GSR) Data and Unit
        /// </summary>
        public readonly SensorData GalvanicSkinResponse;
        /// <summary>
        /// PhotoPlethysmoGram (PPG) Data and Unit
        /// </summary>
        public readonly SensorData PhotoPlethysmoGram;

        /// <summary>
        /// Heart Rate in BPM
        /// </summary>
        public readonly int HeartRate;

        #endregion

        /// <summary>
        /// Default constructor, all parameters are of type SensorData which contains the value and the measuring unit
        /// </summary>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="intTimestamp">Sensor internal timestamp</param>
        /// <param name="acceleratorX">Accelerator X axis</param>
        /// <param name="acceleratorY">Accelerator Y axis</param>
        /// <param name="acceleratorZ">Accelerator Z axis</param>
        /// <param name="galvanicSkinResponse">Galvanic Skip Response (GSR)</param>
        /// <param name="photoPlethysmoGram">PhotoPlethysmoGram (PPG)</param>
        /// <param name="heartRate">Heart Rate in BPM</param>
        public ShimmerGSRData(long timestamp, SensorData intTimestamp, SensorData acceleratorX, SensorData acceleratorY, SensorData acceleratorZ, SensorData galvanicSkinResponse, SensorData photoPlethysmoGram, int heartRate)
        {
            Timestamp = timestamp;
            IntTimestamp = intTimestamp;
            AcceleratorX = acceleratorX;
            AcceleratorY = acceleratorY;
            AcceleratorZ = acceleratorZ;
            GalvanicSkinResponse = galvanicSkinResponse;
            PhotoPlethysmoGram = photoPlethysmoGram;
            HeartRate = heartRate;
        }

        public override string ToString()
        {
            string comma = ",";

            StringBuilder sb = new StringBuilder();
            sb.Append(Timestamp);
            sb.Append(comma);
            sb.Append(IntTimestamp.Data);
            sb.Append(comma);
            sb.Append(AcceleratorX.Data);
            sb.Append(comma);
            sb.Append(AcceleratorY.Data);
            sb.Append(comma);
            sb.Append(AcceleratorZ.Data);
            sb.Append(comma);
            sb.Append(GalvanicSkinResponse.Data);
            sb.Append(comma);
            sb.Append(PhotoPlethysmoGram.Data);
            sb.Append(comma);
            sb.Append(HeartRate);

            return sb.ToString();
        }
    }
}
