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
        
        private readonly List<int> indicesToRemove = new(); 
        private readonly Stopwatch stopwatch = new();
        private int stageLevel;
        private ReactiveProperty<int> score;
        private int disposedIceCount;
        private bool isGameOver = false;
        private bool gameStarted = false;
        private int intervalMilliseconds;
        
        private readonly Subject<Unit> onChangeStageLevel = new();
        private readonly Subject<Unit> onGameOver = new();
        public ReadOnlyReactiveProperty<int> Score => score;
        public Observable<Unit> OnChangeStageLevel => onChangeStageLevel;
        public Observable<Unit> OnGameOver => onGameOver;

        public void Initialize()
        {
            stageLevel = 1;
            disposedIceCount = 0;
            score.Value = 0;
            isGameOver = false;
            gameStarted = false;
            intervalMilliseconds = CalculateInterval(stageLevel);
        }

        public void Tick()
        {
            if(!gameStarted || isGameOver) return;
            
            if (stopwatch.ElapsedMilliseconds >= intervalMilliseconds)
            {
                CreateIce();
                stopwatch.Restart();
            }
            
            for (int i = 0; i < ViewIceDataList.Count; i++)
            {
                var iceData = ViewIceDataList[i];
                if (iceData.Life.CurrentValue <= 0)
                {
                    indicesToRemove.Add(i);
                }
            }
            
            foreach (var index in indicesToRemove)
            {
                RemoveIce(index);
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
            var data = ViewIceDataList[index];
            score.Value += data.Life.CurrentValue;
            ViewIceDataList.RemoveAt(index);
            disposedIceCount++;

            if (disposedIceCount >= 5)
            {
                isGameOver = true;
                onGameOver.OnNext(Unit.Default);
            }
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