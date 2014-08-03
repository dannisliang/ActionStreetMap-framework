namespace Mercraft.Explorer.Messaging
{
    public interface IMessageBus
    {
        void Subscribe<T>(IMessageConsumer<T> consumer) where T : class;
        void Publish<T>(T message) where T : class;
    }
}
