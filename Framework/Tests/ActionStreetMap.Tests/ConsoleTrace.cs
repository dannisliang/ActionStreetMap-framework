using System;
using ActionStreetMap.Infrastructure.Diagnostic;

namespace ActionStreetMap.Tests
{
    public class ConsoleTrace: DefaultTrace
    {
        protected override void WriteRecord(RecordType type, string category, string message, Exception exception)
        {
            Console.WriteLine("[{0}] {1}: {2}{3}", type, category, message,
                (exception == null? "": " .Exception:" + exception));
        }
    }
}
