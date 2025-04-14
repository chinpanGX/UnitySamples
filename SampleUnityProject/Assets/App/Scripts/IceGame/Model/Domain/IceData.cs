using R3;

namespace App.IceGame.Domain
{
    public class IceData
    {
        public readonly int Id;
        private readonly ReactiveProperty<int> life; // アイスの残りライフがスコアになる
        public ReadOnlyReactiveProperty<int> Life => life; 
        
        public IceData(int id)
        {
            if (id < 0)
                throw new System.ArgumentOutOfRangeException(nameof(id), "Id must be non-negative.");
            Id = id;
            life = new ReactiveProperty<int>(100); // 初期ライフを100に設定
        }
        
        public bool Damage()
        {
            life.Value -= 1; // 1秒ごとにライフを減少
            if (life.Value <= 0)
            {
                life.Value = 0; // ライフが0未満にならないようにする
                return true; // アイスが消えた
            }
            return false; // アイスはまだ存在する
        }
        
        public string GetAssetPath()
        {
            return $"icecream_{Id}";
        }
    }
}