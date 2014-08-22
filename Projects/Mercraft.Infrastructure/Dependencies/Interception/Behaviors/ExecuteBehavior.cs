using System.Linq;

namespace Mercraft.Infrastructure.Dependencies.Interception.Behaviors
{
    /// <summary>
    /// Executes method
    /// </summary>
    public class ExecuteBehavior: IBehavior
    {
        public ExecuteBehavior()
        {
            Name = "execute";
        }

        public string Name { get; protected set; }

        /// <summary>
        /// Executes method
        /// </summary>
        public virtual IMethodReturn Invoke(MethodInvocation methodInvocation)
        {
            if (!methodInvocation.IsInvoked)
            {
                var methodBase = TypeHelper.GetMethodBySign(methodInvocation.Target.GetType(),
                    methodInvocation.MethodBase, methodInvocation.GenericTypes.ToArray());
                var result = methodBase.Invoke(methodInvocation.Target, methodInvocation.Parameters.Values.ToArray());
                methodInvocation.IsInvoked = true;
                return methodInvocation.Return = new MethodReturn(result);
            }
            return methodInvocation.Return;
        }
    }
}
