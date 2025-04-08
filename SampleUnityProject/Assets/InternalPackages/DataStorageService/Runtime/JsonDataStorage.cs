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
            var jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(dataFullPath, jsonData, Encoding.UTF8);
        }
        
        public T Load<T>()
        {
            if (!File.Exists(dataFullPath))
                throw new FileNotFoundException("ファイルが見つかりません。", dataFullPath);
            
            if (!File.Exists(dataFullPath))
                throw new FileNotFoundException("ファイルが見つかりません。", dataFullPath);
            
            var jsonData = File.ReadAllText(dataFullPath, Encoding.UTF8);
            var data = JsonConvert.DeserializeObject<T>(jsonData);
            if (data == null)
                throw new InvalidDataException("データの読み込みに失敗しました。");

            return data;
        }
    }
}