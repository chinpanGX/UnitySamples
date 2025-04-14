using System.Collections.Generic;
using System.Diagnostics;
using App.IceGame.Domain;
using AppCore.Runtime;
using ObservableCollections;
using R3;
using UnityEngine;

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

        private readonly Subject<Unit> onChangeStageLevel = new();
        private readonly Subject<Unit> onGameOver = new();
        public Observable<int> ScoreAsObservable => score.Skip(1);
        public Observable<Unit> OnChangeStageLevel => onChangeStageLevel;
        public Observable<Unit> OnGameOver => onGameOver;

        public IceGameModel()
        {
            stageLevel = 1;
            disposedIceCount = 0;
            isGameOver = false;
            gameStarted = false;
            intervalMilliseconds = CalculateInterval(stageLevel);
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
                RemoveIce(0);
                stopwatch.Restart();
            }
        }

        public void Dispose()
        {
            stopwatch.Stop();
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

        public void RemoveIce(int index)
        {
            ViewIceDataList.RemoveAt(index);

            // disposedIceCount++;
            //
            // if (disposedIceCount >= 5)
            // {
            //     isGameOver = true;
            //     onGameOver.OnNext(Unit.Default);
            // }
        }

        public void AddScore(IceData data)
        {
            score.Value += data.Life.CurrentValue;
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
            ViewIceDataList.Add(new IceData(randomId));
        }
    }
}