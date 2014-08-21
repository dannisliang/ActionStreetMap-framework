using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mercraft.Infrastructure.Dependencies.Interception.Behaviors;

namespace Mercraft.Infrastructure.Dependencies.Interception
{
    /// <summary>
    /// Provides base functionality to proxy object
    /// </summary>
    public class ProxyBase: IProxy
    {
        private readonly LinkedList<IBehavior> _behaviors = new LinkedList<IBehavior>();

        public object Instance { get;  set; }

        /// <summary>
        /// add behavior to behaviors' chain
        /// </summary>
        /// <param name="behavior"></param>
        public void AddBehavior(IBehavior behavior)
        {
            // NOTE due to the current IContainer implementation we should check this
            if(!_behaviors.Contains(behavior))
                _behaviors.AddLast(behavior);
        }

        /// <summary>
        /// Build IMethodInvocation object from executed method
        /// </summary>
        /// <param name="methodBase"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected MethodInvocation BuildMethodInvocation(MethodBase methodBase, params object[] args)
        {
            MethodInvocation methodInvocation = new MethodInvocation() { MethodBase = methodBase, Target = Instance };
            var parameters = methodBase.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
                methodInvocation.Parameters.Add(parameters[i], args[i]);
            return methodInvocation;

        }

        /// <summary>
        /// Run behaviors' chain
        /// </summary>
        /// <param name="methodInvocation"></param>
        /// <returns></returns>
        protected IMethodReturn RunBehaviors(MethodInvocation methodInvocation)
        {
            IMethodReturn methodReturn = null;
            return _behaviors.Aggregate(methodReturn, (ac, b) => b.Invoke(methodInvocation));
        }

        /// <summary>
        /// Clears list of behaviors
        /// </summary>
        public void ClearBehaviors()
        {
            _behaviors.Clear();
        }
    }
}
