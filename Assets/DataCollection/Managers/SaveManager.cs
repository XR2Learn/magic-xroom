using System.IO;
using DataCollection.Data;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace DataCollection.Managers
{
    public sealed class SaveManager
    {
        private static SaveManager instance;
        private readonly string delimiter;
        private readonly string extension;
        private readonly string folderName;
        private uint counter;
        private string fileName;
        private string pathName;

        private SaveManager()
        {
            delimiter = "\\";
            pathName = Application.persistentDataPath;
            fileName = "data_collection_VR_";
            counter = 0;
            extension = ".json";
            folderName = "DataCollection";
        }

        public static SaveManager Instance
        {
            get { return instance ??= new SaveManager(); }
        }

        public bool SetCustomSaveFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            if (string.IsNullOrWhiteSpace(fileName)) return false;

            this.fileName = fileName;

            return true;
        }

        public bool SetCustomSavePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
#if DEBUG
                Debug.LogWarning("Path not valid");
#endif
                return false;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
#if DEBUG
                Debug.LogError("Path not valid");
#endif
                return false;
            }

            if (!Directory.Exists(path))
            {
#if DEBUG
                Debug.LogWarning("Path not valid");
#endif
                return false;
            }

            try
            {
                using (var fileStream = File.Create(Path.Combine(path, Path.GetRandomFileName()), 1,
                    FileOptions.DeleteOnClose))
                {
                }
            }
            catch
            {
#if DEBUG
                Debug.LogWarning("Path is not writable");
#endif
                return false;
            }

            pathName = path;

            return true;
        }

        public bool Save(IDatable collection)
        {
            if (collection == null)
            {
#if DEBUG
                Debug.LogWarning("Cannot save NUll object");
#endif
                return false;
            }

            if (collection.IsEmpty())
            {
#if DEBUG
                Debug.LogWarning("Cannot save empty collection");
#endif
                return false;
            }

            string directory = pathName + delimiter + folderName + delimiter;

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string file = fileName + counter + extension;

            while (File.Exists(directory+file)) file = file.Replace(counter.ToString(), (++counter).ToString());

            string json = JsonConvert.SerializeObject(collection);

            using (var streamWriter = new StreamWriter(directory+file))
            {
                streamWriter.WriteLine(json);
            }
#if DEBUG
            Debug.Log(string.Format("File {0} saved to {1}", fileName + counter + extension, pathName + delimiter + folderName));
#endif
            return true;
        }
    }
}