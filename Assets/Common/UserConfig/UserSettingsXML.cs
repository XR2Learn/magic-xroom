
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
            public const string AutoStart = "autoStart";
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