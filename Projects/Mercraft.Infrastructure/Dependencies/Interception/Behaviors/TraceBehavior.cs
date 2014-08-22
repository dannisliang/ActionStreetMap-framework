using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mercraft.Infrastructure.Diagnostic;

namespace Mercraft.Infrastructure.Dependencies.Interception.Behaviors
{
    /// <summary>
    ///     This behavior logs methods signature and result call to output
    /// </summary>
    public class TraceBehavior: ExecuteBehavior
    {
        private readonly ITrace _trace;

        public TraceBehavior(ITrace trace)
        {
            _trace = trace;
            Name = "trace";
        }

        public override IMethodReturn Invoke(MethodInvocation methodInvocation)
        {
            var methodName = String.Format("{0}.{1}({2})", 
                methodInvocation.Target.GetType(),
                methodInvocation.MethodBase.Name,
                methodInvocation.Parameters.Aggregate("", 
                    (ag, p) => ag + (ag != "" ? ", ": "") +
                    (p.Value != null? p.ToString(): "<null>")));

            _trace.Normal("Interception." + Name, String.Format("invoke {0}", methodName));
            
            var result = methodInvocation.IsInvoked? methodInvocation.Return :  
                base.Invoke(methodInvocation);

            var resultString = "";
            var methodInfo = methodInvocation.MethodBase as MethodInfo;
            if (methodInfo != null && methodInfo.ReturnType != typeof(void))
                resultString += ": " + result.GetReturnValue();

            _trace.Normal("Interception." + Name, String.Format("end {0}{1}", methodName, resultString));

            return result;
        }
    }
}
