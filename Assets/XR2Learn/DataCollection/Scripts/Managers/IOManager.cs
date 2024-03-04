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


using XR2Learn.DataCollection.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static XR2Learn.DataCollection.Data.UserEvents;
using XR2Learn.Common.UserConfig;
using XR2Learn.DataCollection.Shimmer;

namespace XR2Learn.DataCollection.Managers
{ 
    public class IOManager
    {
        public enum Sensor
        {
            VR, SHIMMER, EYE, FACE, PROGRESS_EVENT
        }

        private static string OutputPath;
        private const string FolderName = "DataCollection";
        private const string FileName = "data_collection";
        private const string Extension = ".csv";

        private static long timestamp = 0;

        private static bool _initialized = false;

        private static void Append(Sensor type, string frame)
        {
            if (!_initialized) return;

            string path = GetFileName(type);
            File.AppendAllText(path, frame + "\n");
        }

        public static void AppendHeader(Sensor type, string header)
        {
            Append(type, header);
        }

        public static void Append(Sensor type, ShimmerGSRData frame)
        {
            Append(type, frame.ToString());
        }

        public static void Append(Sensor type, VRData frame)
        {
            Append(type, frame.ToString());
        }
    
        public static void Append(Sensor type, List<ShimmerGSRData> frames)
        {
            Append(type, string.Join("\n", frames));
        }

        public static void Append(Sensor type, List<VRData> frames)
        {
            Append(type, string.Join("\n", frames));
        }

        public static void Append(Sensor type, List<EyeData> eyes)
        {
            Append(type, string.Join("\n", eyes));
        }

        public static void Append(Sensor type, List<LipData> faces)
        {
            Append(type, string.Join("\n", faces));
        }

        public static void Append(Sensor type, Level evt, int level)
        {
            Append(type, DateTime.Now.Ticks + "," + evt.ToString() + "," + level);
        }

        public static void Append(Sensor type, Scenario evt, string scenario)
        {
            long now = DateTime.Now.Ticks;
            Append(type, now + "," + evt.ToString() + "," + scenario);
            if (ShimmerGSRManager.Instance != null && ShimmerGSRManager.Instance.IsEnabled())
                Append(type, now + ",SHIMMER," + ShimmerGSRManager.Instance.GetLatestData().IntTimestamp.Data);
        }

        public static void Append(Sensor type, Teleport evt, string scenario)
        {
            Append(type, DateTime.Now.Ticks + "," + evt.ToString() + "," + scenario);
        }

        public static void Append(Sensor type, Feedback feedback, long ms)
        {
            Append(type, DateTime.Now.Ticks + "," + feedback.ToString() + "," + ms);
        }

        public static string GetFileName(Sensor type)
        {
            string dir = Path.Combine(OutputPath, FolderName);
            return Path.Combine(dir, FileName + "_" + timestamp + "_" + type.ToString() + "_" + Extension);
        }

        public static void Init()
        {
            timestamp = DateTime.Now.Ticks;

            UserSettingsLoader.Load(UserSettingsLoader.userSettings.DataCollection_OutputPath, out OutputPath, "");

            string dir = Path.Combine(OutputPath, FolderName);
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                Debug.Log("Data Collection Output Folder: \"" + dir + "\"");
            }
            catch (Exception e)
            {

                dir = Path.Combine(Application.persistentDataPath, FolderName);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                Debug.Log(e.Message + ", Using application persistent data path : \"" + dir + "\"");
            }

            _initialized = true;
        }
    }
}