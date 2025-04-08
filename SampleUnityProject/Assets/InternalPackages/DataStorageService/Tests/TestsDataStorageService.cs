using System;
using System.IO;
using DataStorageService.Runtime;
using NUnit.Framework;
using Newtonsoft.Json;
using UnityEngine;

namespace DataStorageService.Tests
{
    [JsonObject]
    public class TestPlayerData
    {
        [JsonProperty] public string PlayerId { get; private set; }
        
        [JsonProperty] public string Name { get; private set; }
        
        public static TestPlayerData CreateNew()
        {
            var id = Guid.NewGuid().ToString();
            var name = "Player";
            return new TestPlayerData(id, name);
        }
        
        public TestPlayerData(string playerId, string name)
        {
            PlayerId = playerId;
            Name = name;
        }
    }

    public class TestPlayerRepository
    {
        private readonly string fileName = "TestPlayerData";
        private readonly IDataStorage dataStorage;
        private readonly string dataPath;
        
        public TestPlayerRepository()
        {
            dataPath = $"{Application.persistentDataPath}/{fileName}";
            dataStorage = new JsonDataStorage(dataPath);
            
            if (!dataStorage.Exists())
            {
                Create();
            }
        }
        
        private void Create()
        {
            var playerData = TestPlayerData.CreateNew();
            dataStorage.Save(playerData);
        }
        
        public void Save(TestPlayerData playerData)
        {
            dataStorage.Save(playerData);
        }
        
        public TestPlayerData Load()
        {
            return dataStorage.Load<TestPlayerData>();
        }
    }
    
    public class TestsDataStorageService
    {
        [Test]
        public void CreateSaveData()
        {
            var repository = new TestPlayerRepository();
            var playerData = repository.Load();
            
            Assert.IsNotNull(playerData);
            Assert.IsNotEmpty(playerData.PlayerId);
            Debug.Log($"PlayerId: {playerData.PlayerId}");
            Assert.IsNotEmpty(playerData.Name);
            Debug.Log($"PlayerName: {playerData.Name}");
        }
    }
}