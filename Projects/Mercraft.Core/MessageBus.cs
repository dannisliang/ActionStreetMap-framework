using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Mercraft.Core
{
    public interface IMessageBus
    {
        void Send<T>(T message);
        IObservable<T> AsObservable<T>();
    }

    public sealed class MessageBus: IMessageBus, IDisposable
    {
        private readonly Subject<object> _messageSubject = new Subject<object>();

        public void Send<T>(T message)
        {
            _messageSubject.OnNext(message);
        }

        public IObservable<T> AsObservable<T>()
        {
            return _messageSubject.OfType<T>();
        }

        public void Dispose()
        {
            _messageSubject.Dispose();
        }
    }
}
