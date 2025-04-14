using MessagePipe;
using R3;

namespace App.IceGame.Domain
{
    public class IceData
    {
        public readonly int Id;
        private readonly ReactiveProperty<int> life; // アイスの残りライフがスコアになる
        private readonly IPublisher<IceDisposerMessage> iceDisposerPublisher;
        public ReadOnlyReactiveProperty<int> Life => life; 
        
        public IceData(int id, IPublisher<IceDisposerMessage> iceDisposerPublisher)
        {
            if (id < 0)
                throw new System.ArgumentOutOfRangeException(nameof(id), "Id must be non-negative.");
            Id = id;
            life = new ReactiveProperty<int>(100); // 初期ライフを100に設定
            this.iceDisposerPublisher = iceDisposerPublisher;
        }
        
        public void ReduceLife()
        {
            life.Value -= 1; // 1秒ごとにライフを減少
            if (life.Value <= 0)
            {
                life.Value = 0; // ライフが0未満にならないようにする
                iceDisposerPublisher.Publish(new IceDisposerMessage(1));
            }
        }
        
        public string GetAssetPath()
        {
            return $"icecream_{Id}";
        }
    }
}