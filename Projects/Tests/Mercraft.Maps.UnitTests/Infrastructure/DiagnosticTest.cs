using System;
using Mercraft.Infrastructure.Diagnostic;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class DiagnosticTest
    {
        [Description("Dummy test shows that we can use methods without any exceptions")]
        [Test]
        public void CanUseTraceMethods()
        {
            using (var trace = new DefaultTrace())
            {
                trace.Normal("normal");
                trace.Normal("category", "Normal");
                trace.Warn("Warn");
                trace.Warn("category", "Warn");
                trace.Input("Input");
                trace.Input("category", "Input");
                trace.Output("Output");
                trace.Output("category", "Output");
                trace.System("System");
                trace.System("category", "System");
                trace.Error("Error", new ArgumentException());
                trace.Error("category", "Error", new ArgumentException());
            }
        }
    }
}