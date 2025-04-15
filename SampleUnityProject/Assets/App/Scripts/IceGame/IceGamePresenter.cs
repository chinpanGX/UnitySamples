using System.Threading;
using App.IceGame.Domain;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using MessagePipe;
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

        public IceGamePresenter(IDirector director, IceGameModel model, IceGameView view, ISubscriber<IceDisposerMessage> iceDisposerSubscriber)
        {
            Director = director;
            Model = model;
            View = view;
            
            iceDisposerSubscriber.Subscribe(message =>
            {
                var disposedCount = Model.AddIceDisposerCount(message.DisposedIceCount);
                View.SetReduceLife(disposedCount - 1);
            }).RegisterTo(cts.Token);
            
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
            Model.OnChangeStageLevel.Subscribe(_ =>
            {
                Debug.Log("ステージレベルアップ");
            }).RegisterTo(cts.Token);
            
            Model.OnGameOver.Subscribe(_ =>
            {
                Debug.Log("ゲームオーバー");
            }).RegisterTo(cts.Token);
            
            Model.ScoreAsObservable.SubscribeAwait(async (value, _) =>
            {
                await View.ScoreElementView.UpdateScore(value);
            }).RegisterTo(cts.Token);

            Model.ViewIceDataList.ObserveAdd().Subscribe(data =>
                {
                    _ = View.CreateIceElement(data.Value, Model.GiveIce);
                }
            ).RegisterTo(cts.Token);
            
            Model.ViewIceDataList.ObserveRemove().Subscribe(data =>
                {
                    Model.AddScore(data.Value);
                    // お客さんがまた注文をする   
                }
            ).RegisterTo(cts.Token);
            
            View.Push();
            View.Open();
            
            OnStartGame().Forget();
        }
        
        private async UniTask OnStartGame()
        {
            // startViewHandle = await StartView.LoadAsync();
            // var startView = startViewHandle.Result.GetComponent<StartView>();
            // startView.Push();
            // startView.Open();
            // await startView.PlayAsync();
            // startView.Pop();
            Model.StartGame();
        }
    }
}