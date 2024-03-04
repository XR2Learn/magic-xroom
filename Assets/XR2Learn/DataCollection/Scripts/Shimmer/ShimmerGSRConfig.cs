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

namespace XR2Learn.DataCollection.Shimmer
{
    public partial class ShimmerGSR
    {
        #region Default values

        public static readonly int      DefaultNumberOfHeartBeatsToAverage = 1;
        public static readonly int      DefaultTrainingPeriodPPG = 10;
        public static readonly double   DefaultLowPassFilterCutoff = 5;
        public static readonly double   DefaultHighPassFilterCutoff = 0.5;
        public static readonly double   DefaultSamplingRate = 10;
        public static readonly bool     DefaultEnableAccelerator = true;
        public static readonly bool     DefaultEnableGSR = true;
        public static readonly bool     DefaultEnablePPG = true;

        #endregion

        #region Properties

        /// <summary>
        /// Number of Heart Beats required to calculate an average
        /// </summary>
        public int NumberOfHeartBeatsToAverage
        {
            get { return _numberOfHeartBeatsToAverage; }
            set { _numberOfHeartBeatsToAverage = value; }
        }
        private int _numberOfHeartBeatsToAverage;

        /// <summary>
        /// PPG algorithm training period in [s]
        /// </summary>
        public int TrainingPeriodPPG
        {
            get { return _trainingPeriodPPG; }
            set { _trainingPeriodPPG = value; }
        }
        private int _trainingPeriodPPG;

        /// <summary>
        /// PPG low-pass filter cutoff in [Hz]
        /// </summary>
        public double LowPassFilterCutoff
        {
            get { return _LowPassFilterCutoff; }
            set { _LowPassFilterCutoff = value; }
        }
        private double _LowPassFilterCutoff;

        /// <summary>
        /// PPG high-pass filter cutoff in [Hz]
        /// </summary>
        public double HighPassFilterCutoff
        {
            get { return _HighPassFilterCutoff; }
            set { _HighPassFilterCutoff = value; }
        }
        private double _HighPassFilterCutoff;

        /// <summary>
        /// Shimmer device internal sampling rate in [Hz]
        /// </summary>
        public double SamplingRate
        {
            get { return _samplingRate; }
            set { _samplingRate = value; }
        }
        private double _samplingRate;

        /// <summary>
        /// Flag to enable/disable the Accelerator sensor
        /// </summary>
        public bool EnableAccelerator
        {
            get { return _enableAccelerator == (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL; }
            set { _enableAccelerator = value ? (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL : 0; }
        }
        private int _enableAccelerator;

        /// <summary>
        /// Flag to enable/disable the GSR sensor
        /// </summary>
        public bool EnableGSR
        {
            get { return _enableGSR == (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR; }
            set { _enableGSR = value ? (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR : 0; }
        }
        private int _enableGSR;

        /// <summary>
        /// Flag to enable/disable the PPG sensor
        /// </summary>
        public bool EnablePPG
        {
            get { return _enablePPG == (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13; }
            set { _enablePPG = value ? (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13 : 0; }
        }
        private int _enablePPG;

        #endregion
    }
}
