using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace DataStorageService.Tests
{
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
        
        [Test]
        public void UpdateSaveData()
        {
            var repository = new TestPlayerRepository();
            var playerData = repository.Load();
            var newName = "NewPlayerName";
            var updatedPlayerData = playerData.UpdateName(newName);
            repository.Save(updatedPlayerData);
            
            var loadedPlayerData = repository.Load();
            Assert.AreEqual(newName, loadedPlayerData.Name);
            Debug.Log($"Updated PlayerName: {loadedPlayerData.Name}");
        }
        
        [Test]
        public void DeleteSaveData()
        {
            var repository = new TestPlayerRepository();
            var playerData = repository.Load();
            File.Delete($"{Application.persistentDataPath}/TestPlayerData.json");
            
            Assert.IsFalse(File.Exists($"{Application.persistentDataPath}/TestPlayerData.json"));
            Debug.Log("Save data deleted successfully.");
        }
        
        [Test]
        public void PlayerNameExceedsMaxLengthThrowsExceptionOnCreation()
        {
            Assert.Throws<TestPlayerException>(() => new TestPlayerData("123", new string('A', 21)));
        }
        
        [Test]
        public void PlayerNameContainsSpacesThrowsExceptionOnCreation()
        {
            Assert.Throws<TestPlayerException>(() => new TestPlayerData("123", "Player Name"));
        }
    
        [Test]
        public void SavePlayerNameContainsNgWordsThrowsException()
        {
            var repository = new TestPlayerRepository();
            var playerData = new TestPlayerData("123", "ちんちん");
            Assert.Throws<TestPlayerException>(() => repository.Save(playerData));

            playerData = new TestPlayerData("123", "うんち");
            Assert.Throws<TestPlayerException>(() => repository.Save(playerData));
            
            playerData = new TestPlayerData("123", "ちんちんぱんぱん");
            Assert.Throws<TestPlayerException>(() => repository.Save(playerData));
            
            playerData = new TestPlayerData("123", "ちんぱん");
            Assert.DoesNotThrow(() => repository.Save(playerData));
        }
        
        
        [TearDown]
        public void TearDown()
        {
            var repository = new TestPlayerRepository();
            repository.Delete();
        }
        
    }
}