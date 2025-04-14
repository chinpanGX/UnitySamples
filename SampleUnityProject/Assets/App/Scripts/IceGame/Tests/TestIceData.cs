using System.Threading;
using NUnit.Framework;
using ObservableCollections;
using R3;
using UnityEngine;

namespace App.IceGame.Tests
{
    public class TestIceData
    {
        [Test]       
        public void TestIceDataCreation() 
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var model = new IceGameModel();
            model.ViewIceDataList.ObserveAdd().Subscribe(data =>
            {
                Debug.Log($"IceData added: index: {data.Index}, ID: {data.Value.Id}, Life: {data.Value.Life.CurrentValue}, AssetPath: {data.Value.GetAssetPath()}");
            }).RegisterTo(cancellationTokenSource.Token);
            
            for (int i = 0; i < 10; i++)
            {
                model.CreateIce();
            }
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}