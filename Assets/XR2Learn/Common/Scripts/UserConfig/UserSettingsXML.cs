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


namespace XR2Learn.Common.UserConfig
{
    public class UserSettingsXML
    {
        public const string fileName = "UserSettings.xml";

        public struct XMLElements
        {
            public const string Settings = "settings";

            public struct Keyboard
            {
                public const string Name = "keyboard";
                public const string EnableShortcuts = "enableShortcuts";
            }

            public struct DataCollection
            {
                public const string Name = "dataCollection";
                public const string OutputPath = "outputPath";
            }

            public struct VR
            {
                public const string Name = "vr";

                public struct Config
                {
                    public const string Name = "config";
                    public const string SamplingRate = "samplingRate";
                    public const string SamplingBufferSize = "samplingBufferSize";
                }
            }

            public struct Shimmer
            {
                public const string Name = "shimmer";
                public const string Enabled = "enabled";
                public const string DeviceName = "deviceName";

                public struct Config
                {
                    public const string Name = "config";
                    public const string HeartbeatsToAverage = "heartbeatsToAverage";
                    public const string TrainingPeriodPPG = "trainingPeriodPPG";
                    public const string SamplingRate = "samplingRate";
                    public const string SamplingBufferSize = "samplingBufferSize";
                }

                public struct Sensors
                {
                    public const string Name = "sensors";
                    public const string EnableAccelerator = "enableAccelerator";
                    public const string EnableGSR = "enableGSR";
                    public const string EnablePPG = "enablePPG";
                }
            }

            public struct EyeTracking
            {
                public const string Name = "eyeTracking";
                public const string Enabled = "enabled";

                public struct Config
                {
                    public const string Name = "config";
                    public const string SamplingRate = "samplingRate";
                    public const string SamplingBufferSize = "samplingBufferSize";
                }
            }

            public struct FaceTracking
            {
                public const string Name = "faceTracking";
                public const string Enabled = "enabled";

                public struct Config
                {
                    public const string Name = "config";
                    public const string SamplingRate = "samplingRate";
                    public const string SamplingBufferSize = "samplingBufferSize";
                }
            }

            public struct Feedback
            {
                public const string Name = "feedback";
                public const string Enabled = "enabled";
                public const string AfterScenario = "afterScenario";
                public const string AfterLevel = "afterLevel";
            }
        }
    }
}