using System;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.Infrastructure
{
    public class UnityConsoleTrace: DefaultTrace
    {
        protected override void WriteRecord(RecordType type, TraceRecord record)
        {
            switch (type)
            {
                case RecordType.Fatal:
                    Debug.LogException(record.Exception);
                    break;
                case RecordType.Error:
                    Debug.LogError(ConvertRecord(record));
                    break;
                case RecordType.Info:
                    Debug.Log(ConvertRecord(record));
                    break;
                case RecordType.Warn:
                    Debug.LogWarning(ConvertRecord(record));
                    break;
            }
        }

        private string ConvertRecord(TraceRecord record)
        {
            var category = record.Category ?? "";
            return String.Format("{0}:{1}", category, record.Message);
        }
    }
}
