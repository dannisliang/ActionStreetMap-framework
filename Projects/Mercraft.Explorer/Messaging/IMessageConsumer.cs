
namespace Mercraft.Explorer.Messaging
{
    public interface IMessageConsumer<in T>
    {
        void Consume(T message);
    }
}
