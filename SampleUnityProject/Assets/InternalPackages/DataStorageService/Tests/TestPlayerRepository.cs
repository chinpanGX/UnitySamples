using System.Collections.Generic;
using System.Linq;
using DataStorageService.Runtime.Core;
using DataStorageService.Runtime.Json;
using DataStorageService.Runtime.Security;

namespace DataStorageService.Tests
{
    internal interface IPlayerRepository
    {
        void Save(TestPlayerData playerData);
        TestPlayerData Load();
        void Delete();
        bool Exists();
    }
    
    internal class TestPlayerRepository : IPlayerRepository
    {
        private readonly string fileName = "TestPlayerData";
        private readonly IDataStorage dataStorage;

        public TestPlayerRepository(bool development)
        {
            if (development)
            {
                dataStorage = new JsonDataStorage(fileName);    
            }
            else
            {
                dataStorage = new SecurityDataStorageService(fileName, "password1234");
            }
            

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
            if (playerData == null)
                throw new TestPlayerException("Player data cannot be null.");
            if (string.IsNullOrEmpty(playerData.PlayerId))
                throw new TestPlayerException("Player ID cannot be null or empty.");
            if (string.IsNullOrEmpty(playerData.Name))
                throw new TestPlayerException("Player name cannot be null or empty.");
            
            var ngWords = new List<string>
            {
                "ちんちん",
                "うんち"
            };
            foreach (var word in ngWords.Where(word => playerData.Name.Contains(word)))
            {
                throw new TestPlayerException($"Player name cannot contain the word '{word}'.");
            }
            

            dataStorage.Save(playerData);
        }

        public TestPlayerData Load()
        {
            return dataStorage.Load<TestPlayerData>();
        }
        
        public void Delete()
        {
            if (dataStorage.Exists())
            {
                dataStorage.Delete();
            }
        }
        
        public bool Exists() => dataStorage.Exists();
    }
}