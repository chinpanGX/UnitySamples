using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace DataStorageService.Runtime
{
    public class JsonDataStorage : IDataStorage
    {
        private readonly string dataFullPath;
        
        public JsonDataStorage(string fileName)
        {
            dataFullPath = $"{DataStorageConfig.DataPath}/{fileName}.json";
        }
        
        public bool Exists()
        {
            return File.Exists(dataFullPath);
        }
        
        public void Save<T>(T data)
        {
            var directory = Path.GetDirectoryName(dataFullPath);
            if (directory == null)
                throw new DirectoryNotFoundException($"Directory not found. {dataFullPath}");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            var jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(dataFullPath, jsonData, Encoding.UTF8);
        }
        
        public T Load<T>()
        {
            if (!File.Exists(dataFullPath))
                throw new FileNotFoundException("File not found.", dataFullPath);
            
            var jsonData = File.ReadAllText(dataFullPath, Encoding.UTF8);
            var data = JsonConvert.DeserializeObject<T>(jsonData);
            if (data == null)
                throw new InvalidDataException("Failed to load data.");

            return data;
        }
        
        public void Delete()
        {
            if  (File.Exists(dataFullPath))  
            {
                File.Delete(dataFullPath);
            }
            else
            {
                throw new FileNotFoundException("File not found.", dataFullPath);
            }
        }
    }
}