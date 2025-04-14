namespace App.IceGame.Domain
{
    public class IceDisposerMessage
    {
        public int DisposedIceCount { get; }
        public IceDisposerMessage(int disposedIceCount)
        {
            DisposedIceCount = disposedIceCount;
        }
    }
}