using DataCollection.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XR2Learn_ShimmerAPI;
using static DataCollection.Data.UserEvents;

namespace DataCollection.Managers
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

        public static void Append(Sensor type, XR2Learn_ShimmerGSRData frame)
        {
            Append(type, frame.ToString());
        }

        public static void Append(Sensor type, VRData frame)
        {
            Append(type, frame.ToString());
        }
    
        public static void Append(Sensor type, List<XR2Learn_ShimmerGSRData> frames)
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
            Append(type, DateTime.Now.Ticks + "," + evt.ToString() + "," + scenario);
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