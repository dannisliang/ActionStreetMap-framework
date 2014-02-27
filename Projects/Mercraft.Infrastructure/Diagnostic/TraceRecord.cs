
using System;

namespace Mercraft.Infrastructure.Diagnostic
{
    public sealed class TraceRecord
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Page { get; set; }
        public Exception Exception { get; set; }
        public Type SourceType { get; set; }

        public TraceRecord()
        {
            Date = DateTime.MinValue;
        }
    }
}
