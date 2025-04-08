using System.Diagnostics;
using System.Threading;
using App.SampleInGame.Domain;
using AppCore.Runtime;
using MessagePipe;
using R3;

namespace App.SampleInGame
{
    public class SampleInGameModel : IModel
    { 
        private static readonly float AddScoreInterval = 3f; // 3秒ごとにスコアの補正値を挙げる
        
        private readonly ReactiveProperty<int> totalScore = new(0);
        private readonly CancellationTokenSource cts = new();
        private readonly Stopwatch addScoreOffsetValueIntervalStopwatch = new();
        private int addScoreOffsetValue = 1; // スコアの補正値
        
        public ReadOnlyReactiveProperty<int> TotalScore => totalScore;
        
        public SampleInGameModel(ISubscriber<BallCollisionMessage> subscriber)
        {
            subscriber.Subscribe(AddScore).RegisterTo(cts.Token);
        }
        
        public void Initialize()
        {
            addScoreOffsetValueIntervalStopwatch.Start();
        }
        
        public void Tick()
        {
            CheckStopwatchRestartInterval();
        }
        
        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }
        
        public BallData FetchGenerateBallData()
        {
            return new BallData(100);
        }
        
        private void AddScore(BallCollisionMessage message)
        {
            totalScore.Value += message.Score + addScoreOffsetValue;
        }
        
        private void CheckStopwatchRestartInterval()
        {
            if (addScoreOffsetValueIntervalStopwatch.Elapsed.TotalSeconds >= AddScoreInterval)
            {
                addScoreOffsetValue += 1;
                addScoreOffsetValueIntervalStopwatch.Restart();
            }
        }
    }
}