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


using System.Threading.Tasks;

namespace XR2Learn.DataCollection.Shimmer
{
    public partial class ShimmerGSR
    {
        /// <summary>
        /// Connects to the Shimmer device
        /// </summary>
        public void Connect()
        {
            if (IsConnected()) return;
            Shimmer.Connect();
        }

        /// <summary>
        /// Disconnects the Shimmer device if connected
        /// </summary>
        public async void Disconnect()
        {
            Shimmer.Disconnect();
            await DelayWork(1000);
            Shimmer.UICallback = null;
        }

        /// <summary>
        /// Tells the Shimmer device to start streaming data
        /// </summary>
        public async void StartStreaming()
        {
            
            await DelayWork(1000);
            Shimmer?.StartStreaming();
        }

        /// <summary>
        /// Tells the Shimmer device to stop streaming data
        /// </summary>
        public async void StopStreaming()
        {
            Shimmer?.StopStreaming();
            await DelayWork(1000);
        }

        /// <summary>
        /// Returns the Shimmer device connection status
        /// </summary>
        /// <returns>True if connected, False otherwise</returns>
        public bool IsConnected()
        {
            return Shimmer.IsConnected();
        }

        /// <summary>
        /// Delays work by 't' milliseconds for the current thread
        /// </summary>
        /// <param name="t">Delay in [ms]</param>
        /// <returns>Handle to the async call</returns>
        private async Task DelayWork(int t)
        {
            await Task.Delay(t);
        }
    }
}
