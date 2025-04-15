using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using App.IceGame.Domain;
using AppCore.Runtime;
using MessagePipe;
using ObservableCollections;
using R3;
using UnityEngine;
using ZLinq;

#if UNITY_EDITOR
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("App.IceGame.Tests")]
#endif

namespace App.IceGame
{
    public class IceGameModel : IModel
    {
        public readonly ObservableList<IceData> ViewIceDataList = new();

        private readonly Stopwatch stopwatch = new();
        private readonly ReactiveProperty<int> score = new(0);
        private int stageLevel;
        private int disposedIceCount; // とけたアイスの数
        private bool isGameOver = false;
        private bool gameStarted = false;
        private int intervalMilliseconds;

        private readonly IPublisher<IceDisposerMessage> iceDisposerPublisher;
        private readonly Subject<Unit> onChangeStageLevel = new();
        private readonly Subject<Unit> onGameOver = new();
        private readonly CancellationTokenSource cancellationTokenSource = new();
        public Observable<int> ScoreAsObservable => score.Skip(1);
        public Observable<Unit> OnChangeStageLevel => onChangeStageLevel;
        public Observable<Unit> OnGameOver => onGameOver;
        
        public IceGameModel(IPublisher<IceDisposerMessage> iceDisposerPublisher)
        {
            stageLevel = 1;
            disposedIceCount = 0;
            isGameOver = false;
            gameStarted = false;
            intervalMilliseconds = CalculateInterval(stageLevel);
            this.iceDisposerPublisher = iceDisposerPublisher;
        }

        public void Initialize()
        {
            
        }
        
        public void Tick()
        {
            if (!gameStarted || isGameOver) return;

            if (stopwatch.ElapsedMilliseconds >= intervalMilliseconds)
            {
                CreateIce();
                stopwatch.Restart();
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        public void StartGame()
        {
            gameStarted = true;

            for (int i = 0; i < 10; i++)
            {
                CreateIce();
            }
            stopwatch.Start();
        }

        public void GiveIce(string uniqueId)
        {
            var iceData = ViewIceDataList.AsValueEnumerable().FirstOrDefault(x => x.UniqueId == uniqueId);
            if (iceData == null)
            {
                UnityEngine.Debug.LogError($"IceData with UniqueId {uniqueId} not found.");
                return;
            }
            AddScore(iceData);
            ViewIceDataList.Remove(iceData);
        }
        

        public void AddScore(IceData data)
        {
            score.Value += data.Life.CurrentValue;
        }
        
        public int AddIceDisposerCount(int count)
        {
            disposedIceCount += count;
            if (disposedIceCount < 5) 
                return disposedIceCount;
            
            isGameOver = true;
            onGameOver.OnNext(Unit.Default);
            return disposedIceCount;
        }

        private void LevelUp()
        {
            stageLevel++;
            intervalMilliseconds = CalculateInterval(stageLevel);
            onChangeStageLevel.OnNext(Unit.Default);
        }

        private int CalculateInterval(int level)
        {
            // ステージレベルに応じて間隔を短くする（例: 最小500ms、最大2000ms）
            return Mathf.Max(500, 2000 - (level - 1) * 100);
        }

        internal void CreateIce()
        {
            var randomId = Random.Range(1, 10);
            ViewIceDataList.Add(new IceData(randomId, iceDisposerPublisher));
        }
    }
}