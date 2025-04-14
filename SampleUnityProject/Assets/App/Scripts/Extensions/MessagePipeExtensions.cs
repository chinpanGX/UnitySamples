using MessagePipe;

namespace App.Extensions
{
    public static class MessagePipeEx
    {
        public static (IPublisher<T> publisher, ISubscriber<T> subscriber) GetMessagePipePubSub<T>()
        {
            var pub = GlobalMessagePipe.GetPublisher<T>();
            var sub = GlobalMessagePipe.GetSubscriber<T>();
            return (pub, sub);
        }
    }
}
