using R3;

namespace App.IceGame.Domain
{
    public class IceData
    {
        public readonly int Id;
        private readonly ReactiveProperty<int> life; // アイスのライフがスコアになる
        public ReadOnlyReactiveProperty<int> Life => life; 
        
        public IceData(int id)
        {
            if (id < 0)
                throw new System.ArgumentOutOfRangeException(nameof(id), "Id must be non-negative.");
            Id = id;
            life = new ReactiveProperty<int>(100); // 初期ライフを100に設定
        }
        
        public void SubtractLife(int damage)
        {
            if (damage < 0)
                throw new System.ArgumentOutOfRangeException(nameof(damage), "Damage must be non-negative.");
            life.Value -= damage;
            if (life.Value < 0)
                life.Value = 0;
        }
    }
}