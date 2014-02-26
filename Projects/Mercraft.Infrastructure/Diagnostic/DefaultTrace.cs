using System;

namespace Mercraft.Infrastructure.Diagnostic
{
    /// <summary>
    /// Empty trace which simplifies logging via inheritance
    /// </summary>
    public class DefaultTrace : ITrace
    {
       
        #region ITrace members
        public void Info(string message)
        {
            this.WriteRecord(RecordType.Info, new TraceRecord() { Message = message });
        }

        public void Info(TraceCategory category, string message)
        {
            this.WriteRecord(RecordType.Info, new TraceRecord() { Category = category, Message = message });
        }

        public void Info(TraceRecord record)
        {
            this.WriteRecord(RecordType.Info, record);
        }

        public void Warn(string message)
        {
            this.WriteRecord(RecordType.Warn, new TraceRecord() { Message = message });
        }

        public void Warn(TraceCategory category, string message)
        {
            this.WriteRecord(RecordType.Warn, new TraceRecord() { Category = category, Message = message });
        }

        public void Warn(TraceRecord record)
        {
            this.WriteRecord(RecordType.Warn, record);
        }

        public void Error(string message, Exception exception)
        {
            this.WriteRecord(RecordType.Error, new TraceRecord() { Message = message, Exception = exception });
        }

        public void Error(TraceCategory category, string message, Exception exception)
        {
            this.WriteRecord(RecordType.Error, new TraceRecord() { Category = category, Message = message, Exception = exception });
        }

        public void Error(TraceRecord record)
        {
            this.WriteRecord(RecordType.Error, record);
        }

        public void Fatal(string message, Exception exception)
        {
            this.WriteRecord(RecordType.Fatal, new TraceRecord() { Message = message, Exception = exception });
        }

        public void Fatal(TraceCategory category, string message, Exception exception)
        {
            this.WriteRecord(RecordType.Fatal, new TraceRecord() { Category = category, Message = message, Exception = exception });
        }

        public void Fatal(TraceRecord record)
        {
            this.WriteRecord(RecordType.Fatal, record);
        }

        public object GetUnderlyingStorage()
        {
            return null;
        }

        /// <summary>
        /// Level of tracing
        /// </summary>
        public int Level { get; set; }

        public bool IsInitialized { get; set; }

        public virtual void Dispose()
        {
            
        }

        #endregion

        protected virtual void WriteRecord(RecordType type, TraceRecord record)
        {
          
        }

        #region nested classes

        protected enum RecordType
        {
            Info,
            Warn,
            Error,
            Fatal
        }

        #endregion

    }
}
