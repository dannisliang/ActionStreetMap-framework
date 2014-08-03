using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercraft.Explorer.Messaging
{
    public class MessageBus : IMessageBus
    {
        private Dictionary<Type, List<WeakReference>> _consumerMap = new Dictionary<Type, List<WeakReference>>();

        public void Subscribe<T>(IMessageConsumer<T> consumer) where T : class
        {
            var type = typeof (T);
            if (!_consumerMap.ContainsKey(type))
                _consumerMap.Add(type, new List<WeakReference>());

            _consumerMap[type].Add(new WeakReference(consumer));
        }

        public void Publish<T>(T message) where T : class
        {
            var type = typeof (T);
            if (!_consumerMap.ContainsKey(type))
                return;

            var hasDead = false;
            foreach (var consumer in _consumerMap[type])
            {
                if (consumer.IsAlive)
                {
                    var target = consumer.Target as IMessageConsumer<T>;
                    if(target != null)
                        target.Consume(message);
                }
                else
                    hasDead = true;
                
            }

            // cleanup dead references
            if (hasDead)
            {
                _consumerMap[type] = _consumerMap[type].Where(c => c.IsAlive).ToList();
            }
        }
    }
}
