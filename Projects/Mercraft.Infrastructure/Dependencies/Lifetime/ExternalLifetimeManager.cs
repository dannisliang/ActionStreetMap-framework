using System;
using ActionStreetMap.Infrastructure.Config;

namespace ActionStreetMap.Infrastructure.Dependencies.Lifetime
{
    /// <summary>
    ///     Wraps already created instance using WeakReference object
    /// </summary>
    internal class ExternalLifetimeManager : ILifetimeManager
    {
        private readonly WeakReference _reference;

        public ExternalLifetimeManager(object instance)
        {
            _reference = new WeakReference(instance);
            TargetType = instance.GetType();
        }

        public Type InterfaceType { get; set; }
        public Type TargetType { get; set; }
        public bool NeedResolveCstorArgs { get; set; }
        public IConfigSection ConfigSection { get; set; }
        public object[] CstorArgs { get; set; }


        /// <summary>
        ///     returns instace if it exists
        /// </summary>
        public object GetInstance()
        {
            if (_reference.IsAlive)
                return _reference.Target;
            throw new InvalidOperationException(
                String.Format("Registeted object is dead! Type: {0}, interface: {1}", TargetType, InterfaceType));
        }

        public object GetInstance(string name)
        {
            return GetInstance();
        }

        public void Dispose()
        {
        }

        public System.Reflection.ConstructorInfo Constructor { get; set; }
    }
}