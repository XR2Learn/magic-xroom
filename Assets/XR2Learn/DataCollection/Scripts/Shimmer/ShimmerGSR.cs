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
using ShimmerLibrary;
using System;

namespace XR2Learn.DataCollection.Shimmer
{
    public partial class ShimmerGSR
    {
        #region Instance variables

        // Shimmer3 device
        private ShimmerLogAndStreamSystemSerialPort Shimmer;

        // Filters and algorithm
        private readonly Filter LPF_PPG; // Low Pass Filter
        private readonly Filter HPF_PPG; // High Pass Filter
        private readonly PPGToHRAlgorithm PPGtoHeartRateCalculation; // PPG to Heartrate

        // Support
        private bool FirstDataPacket = true;
        private int LastKnownState = (int)ShimmerBluetooth.SHIMMER_STATE_NONE;

        // The index of the signals originating from ShimmerBluetooth
        private int IndexTimeStamp;
        private int IndexAcceleratorX;
        private int IndexAcceleratorY;
        private int IndexAcceleratorZ;
        private int IndexGSR;
        private int IndexPPG;

        public ShimmerGSRData LatestData { get; private set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShimmerGSR()
        {
            // Device properties
            _numberOfHeartBeatsToAverage = DefaultNumberOfHeartBeatsToAverage;
            _trainingPeriodPPG = DefaultTrainingPeriodPPG;
            _LowPassFilterCutoff = DefaultLowPassFilterCutoff;
            _HighPassFilterCutoff = DefaultHighPassFilterCutoff;
            _samplingRate = DefaultSamplingRate;

            // Sensors
            _enableAccelerator = (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL;
            _enableGSR = (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR;
            _enablePPG = (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13;

            // Algorithm and filters
            PPGtoHeartRateCalculation = new PPGToHRAlgorithm(_samplingRate, _numberOfHeartBeatsToAverage, _trainingPeriodPPG);
            LPF_PPG = new Filter(Filter.LOW_PASS, _samplingRate, new double[] { _LowPassFilterCutoff });
            HPF_PPG = new Filter(Filter.HIGH_PASS, _samplingRate, new double[] { _HighPassFilterCutoff });
        }

        /// <summary>
        /// Configures the Shimmer device
        /// </summary>
        /// <param name="deviceName">User Defined Device Name</param>
        /// <param name="comPort">Bluetooth Com Port</param>
        public void Configure(string deviceName, string comPort)
        {
            // Setup PPG-to-HR filters and algorithm
            PPGtoHeartRateCalculation.setParameters(_samplingRate, _numberOfHeartBeatsToAverage, _trainingPeriodPPG);
            LPF_PPG.SetFilterParameters(Filter.LOW_PASS,  _samplingRate, new double[] { _LowPassFilterCutoff }, Filter.defaultNTaps);
            HPF_PPG.SetFilterParameters(Filter.HIGH_PASS, _samplingRate, new double[] { _HighPassFilterCutoff }, Filter.defaultNTaps);

            // Define enabled sensors
            int enabledSensors = (
                (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_A_ACCEL & _enableAccelerator |  /// Accelerator
                (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR     & _enableGSR |          /// Galvanic Skin Response
                (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13 & _enablePPG            /// PPG (photoplethysmogram)
            );

            Shimmer = new ShimmerLogAndStreamSystemSerialPort(
                deviceName,                                         /// User Defined Device Name
                comPort,                                            /// Bluetooth Com Port
                _samplingRate,                                      /// Sampling rate in Hz
                0,                                                  /// Accelerator range; 0,1,2,3,4 = 2g,4g,8g,16g.
                ShimmerBluetooth.GSR_RANGE_AUTO,                    /// Range is between 0 and 4. 0 = 10-56kOhm, 1 = 56-220kOhm, 2 = 220-680kOhm, 3 = 680kOhm-4.7MOhm, 4 = Auto range
                enabledSensors,                                     /// Enabled sensors
                false,                                              /// enableLowPowerAccel
                false,                                              /// enableLowPowerGyro
                false,                                              /// enableLowPowerMag
                1,                                                  /// Gyroscope range; Options are 0,1,2,3. Where 0 = 250 Degree/s, 1 = 500 Degree/s, 2 = 1000 Degree/s, 3 = 2000 Degree/s
                0,                                                  /// Magnetometer range; 1,2,3,4,5,6,7 = 1.3, 1.9, 2.5, 4.0, 4.7, 5.6, 8.1
                Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP1,  /// exg1configuration
                Shimmer3Configuration.EXG_EMG_CONFIGURATION_CHIP2,  /// exg2configuration
                true                                                /// internalExpPower
            );

            // Define event callback
            Shimmer.UICallback += this.HandleEvent;
        }

        /// <summary>
        /// Handles stream callback event
        /// </summary>
        /// <param name="sender">Entity sending the callback</param>
        /// <param name="args">Callback parameters</param>
        private void HandleEvent(object sender, EventArgs args)
        {
            CustomEventArgs eventArgs = (CustomEventArgs)args;
            int indicator = eventArgs.getIndicator();

            switch (indicator)
            {
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_STATE_CHANGE:
                    System.Diagnostics.Debug.Write(((ShimmerBluetooth)sender).GetDeviceName() + " State = " + ((ShimmerBluetooth)sender).GetStateString() + System.Environment.NewLine);
                    LastKnownState = (int)eventArgs.getObject();
                    if (LastKnownState == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                    {
                        Console.WriteLine("Shimmer is Connected");
                    }
                    else if (LastKnownState == ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                    {
                        Console.WriteLine("Establishing Connection to Shimmer Device");
                    }
                    else if (LastKnownState == ShimmerBluetooth.SHIMMER_STATE_NONE)
                    {
                        Console.WriteLine("Shimmer is Disconnected");
                    }
                    else if (LastKnownState == ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                    {
                        Console.WriteLine("Shimmer is Streaming");
                    }
                    break;

                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                    ObjectCluster objectCluster = (ObjectCluster)eventArgs.getObject();
                    if (FirstDataPacket)
                    {
                        IndexTimeStamp    = objectCluster.GetIndex(ShimmerConfiguration.SignalNames.SYSTEM_TIMESTAMP, ShimmerConfiguration.SignalFormats.CAL);
                        IndexAcceleratorX = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_X, ShimmerConfiguration.SignalFormats.CAL);
                        IndexAcceleratorY = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Y, ShimmerConfiguration.SignalFormats.CAL);
                        IndexAcceleratorZ = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.LOW_NOISE_ACCELEROMETER_Z, ShimmerConfiguration.SignalFormats.CAL);
                        IndexGSR          = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.GSR, ShimmerConfiguration.SignalFormats.CAL);
                        IndexPPG          = objectCluster.GetIndex(Shimmer3Configuration.SignalNames.INTERNAL_ADC_A13, ShimmerConfiguration.SignalFormats.CAL);

                        FirstDataPacket = false;
                    }

                    //Process PPG signal and calculate heart rate
                    double dataFilteredLP = LPF_PPG.filterData(objectCluster.GetData(IndexPPG).Data);
                    double dataFilteredHP = HPF_PPG.filterData(dataFilteredLP);
                    int heartRate = (int)Math.Round(PPGtoHeartRateCalculation.ppgToHrConversion(dataFilteredHP, objectCluster.GetData(IndexTimeStamp).Data));

                    LatestData = new ShimmerGSRData(
                        DateTime.Now.Ticks,
                        objectCluster.GetData(IndexTimeStamp),
                        objectCluster.GetData(IndexAcceleratorX),
                        objectCluster.GetData(IndexAcceleratorY),
                        objectCluster.GetData(IndexAcceleratorZ),
                        objectCluster.GetData(IndexGSR),
                        objectCluster.GetData(IndexPPG),
                        heartRate
                    );
                    break;
            }
        }
    }
}
