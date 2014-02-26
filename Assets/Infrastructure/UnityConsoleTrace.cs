
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Infrastructure
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
                    Debug.LogError(record);
                    break;
                case RecordType.Info:
                    Debug.Log(record);
                    break;
                case RecordType.Warn:
                    Debug.LogWarning(record);
                    break;
            }
        }
    }
}
