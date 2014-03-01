using System;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies.Interception;

namespace Mercraft.Infrastructure.Dependencies.Lifetime
{
    /// <summary>
    /// Creates singleton instance for wrapped type
    /// </summary>
    public class SingletonLifetimeManager : ILifetimeManager
    {
        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public bool NeedResolveCstorArgs { get; set; }
        public IConfigSection ConfigSection { get; set; }
        public object[] CstorArgs { get; set; }
        public System.Reflection.ConstructorInfo Constructor { get; set; }

        public SingletonLifetimeManager()
        {
            
        }

        private object _instance;
        private IProxy _proxy;

        /// <summary>
        /// Returns singleton instance
        /// </summary>
        /// <returns></returns>
        public object GetInstance()
        {
            return GetInstance(String.Empty);
        }

        /// <summary>
        /// returns new instance of the target type. The name parameters isn't used
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetInstance(string name)
        {
            object target = _instance;
            if (_instance == null)
            {
                _instance = (Constructor ?? TypeHelper.GetConstructor(TargetType, CstorArgs))
                    .Invoke(CstorArgs);
                _proxy = InterceptionContext.CreateProxy(InterfaceType, _instance);

                target = _proxy ?? _instance;
                var configurable = target as IConfigurable;
                if (configurable != null && ConfigSection != null)
                {
                    configurable.Configure(ConfigSection);
                }
            }

            return target;
        }

        public void Dispose()
        {
            if (_instance is IDisposable)
                (_instance as IDisposable).Dispose();
            _instance = null;
        }

        
    }
}
