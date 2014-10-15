using System;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies.Interception;

namespace Mercraft.Infrastructure.Dependencies.Lifetime
{
    /// <summary>
    ///     Creates singleton instance for wrapped type
    /// </summary>
    public class SingletonLifetimeManager : ILifetimeManager
    {
        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public bool NeedResolveCstorArgs { get; set; }
        public IConfigSection ConfigSection { get; set; }
        public object[] CstorArgs { get; set; }
        public System.Reflection.ConstructorInfo Constructor { get; set; }

        private object _instance;
        private IProxy _proxy;

        /// <summary>
        ///     Returns singleton instance
        /// </summary>
        public object GetInstance()
        {
            return GetInstance(String.Empty);
        }

        /// <summary>
        ///     Returns new instance of the target type. The name parameters isn't used
        /// </summary>
        public object GetInstance(string name)
        {
            object target = _proxy ?? _instance;
            if (_instance == null)
            {
                _instance = (Constructor ?? TypeHelper.GetConstructor(TargetType, CstorArgs))
                    .Invoke(CstorArgs);
                _proxy = InterceptionContext.CreateProxy(InterfaceType, _instance);

                var configurable = _instance as IConfigurable;
                if (configurable != null && ConfigSection != null)
                {
                    configurable.Configure(ConfigSection);
                }

                target = _proxy ?? _instance;
            }

            return target;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_instance is IDisposable)
                    (_instance as IDisposable).Dispose();
                _instance = null;
            }
        }
    }
}