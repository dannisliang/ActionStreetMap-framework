using System;
using System.Diagnostics;
using System.Linq;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Infrastructure.Dependencies.Interception.Behaviors
{
    public class ProfileBehavior: IBehavior
    {
        private readonly ITrace _trace;
        public string Name { get; private set; }

        public ProfileBehavior(ITrace trace)
        {
            _trace = trace;
            Name = "profile";
        }

        public IMethodReturn Invoke(MethodInvocation methodInvocation)
        {
            var methodName = String.Format("{0}.{1}({2})",
                methodInvocation.Target.GetType(),
                methodInvocation.MethodBase.Name,
                methodInvocation.Parameters.Aggregate("",
                    (ag, p) => ag + (ag != "" ? ", " : "") +
                    (p.Value != null ? p.ToString() : "<null>")));

            if (methodInvocation.IsInvoked)
            {
                _trace.Warn("Interception." + Name, 
                    String.Format("Unable to profile {0} as method is already executed! " +
                                  "Please check your interception behavior configuration", methodName));
                return methodInvocation.Return;
            }

            var methodBase = TypeHelper.GetMethodBySign(methodInvocation.Target.GetType(),
                   methodInvocation.MethodBase, methodInvocation.GenericTypes.ToArray());

            var sw = new Stopwatch();
            sw.Start();
            var result = methodBase.Invoke(methodInvocation.Target, methodInvocation.Parameters.Values.ToArray());
            sw.Stop();
            _trace.Normal("Interception." + Name, String.Format("{0}: {1} ms", methodName, sw.ElapsedMilliseconds));
            
            methodInvocation.IsInvoked = true;
            return methodInvocation.Return = new MethodReturn(result);
        }
    }
}
