using System;

namespace Mercraft.Infrastructure.Diagnostic
{
    /// <summary>
    /// Empty trace which simplifies logging via inheritance
    /// </summary>
    public class DefaultTrace : ITrace
    {
        #region ITrace implementation

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        public int Level { get; set; }
        public void Normal(string message)
        {
            WriteRecord(RecordType.Normal, "", message, null);
        }

        public void Normal(string category, string message)
        {
            WriteRecord(RecordType.Normal, category, message, null);
        }

        public void Output(string message)
        {
            WriteRecord(RecordType.Output, "", message, null);
        }

        public void Output(string category, string message)
        {
            WriteRecord(RecordType.Normal, category, message, null);
        }

        public void Input(string message)
        {
            WriteRecord(RecordType.Input, "", message, null);
        }

        public void Input(string category, string message)
        {
            WriteRecord(RecordType.Normal, category, message, null);
        }

        public void System(string message)
        {
            WriteRecord(RecordType.System, "", message, null);
        }

        public void System(string category, string message)
        {
            WriteRecord(RecordType.Normal, category, message, null);
        }

        public void Warn(string message)
        {
            WriteRecord(RecordType.Warning, "", message, null);
        }

        public void Warn(string category, string message)
        {
            WriteRecord(RecordType.Normal, category, message, null);
        }

        public void Error(string message, Exception exception)
        {
            WriteRecord(RecordType.Error, "", message, exception);
        }

        public void Error(string category, string message, Exception exception)
        {
            WriteRecord(RecordType.Normal, category, message, exception);
        }

        #endregion

        protected virtual void WriteRecord(RecordType type, string category, string message, Exception exception)
        {

        }
    }
}
