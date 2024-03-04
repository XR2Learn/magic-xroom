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


using System;
using System.Threading;

namespace XR2Learn.DataCollection.Shimmer
{
    /// <summary>
    /// Sample class showcasing a simplified use case
    /// </summary>
    public class Sample
    {
        private static readonly ShimmerGSR api = new ShimmerGSR();

        private static Timer timer; // keep global or it will be garbage-collected after few seconds
        private static int t = 60000; //ms
        private static string deviceName = "Shimmer3";

        public static void Main(string[] args)
        {
            string[] serialPorts = SerialPortsManager.GetAvailableSerialPortsNames();
            Console.WriteLine("Available ports: [ " + string.Join(", ", serialPorts) + " ]");
            foreach (string serialPort in serialPorts)
            {
                api.Configure(deviceName, serialPort);

                api.NumberOfHeartBeatsToAverage = ShimmerGSR.DefaultNumberOfHeartBeatsToAverage;
                api.TrainingPeriodPPG           = ShimmerGSR.DefaultTrainingPeriodPPG;
                api.LowPassFilterCutoff         = ShimmerGSR.DefaultLowPassFilterCutoff;
                api.HighPassFilterCutoff        = ShimmerGSR.DefaultHighPassFilterCutoff;
                api.SamplingRate                = ShimmerGSR.DefaultSamplingRate;

                api.EnableAccelerator = true;
                api.EnableGSR = true;
                api.EnablePPG = true;

                Console.WriteLine("Attempting connection on serial port " + serialPort + " ...");
                api.Connect();
            }

            if (api.IsConnected())
            { 
                Console.WriteLine("Device connected");

                Console.WriteLine("Sending StartStreaming message");
                api.StartStreaming();
                Console.WriteLine("Receiving data for " + (t / 1000) + "s ...");

                timer = HandleData(1); // 1Hz
                WaitAndDisconnect(t);
            }
            else
            {
                Console.WriteLine("Unable to connect to device");
            }
        }

        private static Timer HandleData(int hz)
        {
            var period = TimeSpan.FromMilliseconds(1000 / hz);

            return new Timer((e) =>
            {
                ShimmerGSRData data = api.LatestData; // get latest dataframe
                if (data == null) return;
                Console.WriteLine("[" + data.Timestamp + "/" + data.IntTimestamp.Data + "] " + data.AcceleratorX.Data + " [" + data.AcceleratorX.Unit + "] | " + data.AcceleratorY.Data + " [" + data.AcceleratorY.Unit + "] | " + data.AcceleratorZ.Data + " [" + data.AcceleratorZ.Unit + "]");
                Console.WriteLine(" " + data.GalvanicSkinResponse.Data + " [" + data.GalvanicSkinResponse.Unit + "] | " + data.PhotoPlethysmoGram.Data + " [" + data.PhotoPlethysmoGram.Unit + "] | " + data.HeartRate + " [BPM]");
            }, null, TimeSpan.Zero, period);
        }

        private static void WaitAndDisconnect(int t)
        {
            Thread.Sleep(t);
            timer.Dispose();

            Console.WriteLine("Sending StopStreaming message");
            api.StopStreaming();
            Console.WriteLine("Disconnecting");
            api.Disconnect();
            Console.WriteLine("Device disconnected");
        }
    }
}
