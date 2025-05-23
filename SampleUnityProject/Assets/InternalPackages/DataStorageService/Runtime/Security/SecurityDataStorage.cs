using System.IO;
using DataStorageService.Runtime.Core;
using DSecurity.Cryptography;
using Newtonsoft.Json;
using UnityEngine;

namespace DataStorageService.Runtime.Security
{
    public class SecurityDataStorageService : IDataStorage
    {
        private readonly string dataFullPath;
        private readonly string bundleId;
        private readonly string password;

        public SecurityDataStorageService(string fileName, string password)
        {
            dataFullPath = $"{DataStorageConfig.DataPath}/{fileName}.bin";
            bundleId = Application.identifier;
            this.password = password;
        }

        public bool Exists()
        {
            return FileHelper.Exists(dataFullPath);
        }

        public void Save<T>(T data)
        {
            FileHelper.CreateDirectoryIfNeed(dataFullPath);

            var encrypted = Executor.Encrypt(JsonConvert.SerializeObject(data), password, bundleId);
            using var fileStream = new FileStream(dataFullPath, FileMode.Create, FileAccess.Write);
            fileStream.Write(encrypted, 0, encrypted.Length);
        }

        public T Load<T>()
        {
            if (!FileHelper.Exists(dataFullPath))
                throw new FileNotFoundException("File not found.", dataFullPath);
            
            using var fileStream = new FileStream(dataFullPath, FileMode.Open, FileAccess.Read);
            var bin = new byte[fileStream.Length];
            _ = fileStream.Read(bin, 0, bin.Length);
            
            var data = JsonConvert.DeserializeObject<T>(Executor.Decrypt(bin, password, bundleId));
            if (data == null)
                throw new InvalidDataException("Failed to load data.");

            return data;
        }

        public void Delete()
        {
            FileHelper.Delete(dataFullPath);
        }
    }
}
