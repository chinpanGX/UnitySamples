using System.Threading;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.IceGame
{
    public class IceGamePresenter : IPresenter
    {
        private IDirector Director { get; set; }
        private IceGameModel Model { get; set; }
        private IceGameView View { get; set; }
        private readonly CancellationTokenSource cts = new();
        private IPresenter presenterImplementation;
        
        private AsyncOperationHandle<GameObject> startViewHandle;

        public IceGamePresenter(IDirector director, IceGameModel model, IceGameView view)
        {
            Director = director;
            Model = model;
            View = view;
            
            Setup();
        }
        
        public void Tick()
        {
            Model.Tick();
        }
        
        public void Dispose()
        {
            if (startViewHandle.IsValid())
                startViewHandle.Release();
            
            View.Pop();
            cts?.Dispose();
            Model?.Dispose();
        }

        private void Setup()
        {
            Model.Initialize();

            Model.OnChangeStageLevel.Subscribe(_ =>
            {
                Debug.Log("ステージレベルアップ");
            }).RegisterTo(cts.Token);
            
            Model.OnGameOver.Subscribe(_ =>
            {
                Debug.Log("ゲームオーバー");
            }).RegisterTo(cts.Token);

            Model.Score.Subscribe(_ =>
            {
                Debug.Log("Score");
            }).RegisterTo(cts.Token);

            Model.ViewIceDataList.ObserveAdd().Subscribe(data =>
                {
                    Debug.Log($"氷が追加されました: {data}");
                }
            ).RegisterTo(cts.Token);
            
            Model.ViewIceDataList.ObserveRemove().Subscribe(data =>
                {
                    Debug.Log($"氷が削除されました: {data}");
                }
            ).RegisterTo(cts.Token);
            
            View.Push();
            View.Open();
            
            OnStartGame().Forget();
        }
        
        private async UniTask OnStartGame()
        {
            startViewHandle = await StartView.LoadAsync();
            var startView = startViewHandle.Result.GetComponent<StartView>();
            startView.Push();
            startView.Open();
            await startView.PlayAsync();
            startView.Pop();
            Model.StartGame();
        }
    }
}