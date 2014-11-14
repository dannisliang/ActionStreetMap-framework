using System;
using System.Text;

namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class Error : NmeaMessage
    {
        public Error()
        {
            Id = NmeaMessageType.Error;
        }

        public Error(string line, Exception ex)
        {
            Id = NmeaMessageType.Error;
            Line = line;
            Exception = ex;
        }

        public Exception Exception = null;
        public string Line = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Id).Append(" ");
            if (Line != null)
                sb.Append(Line).Append(" ");

            if (Exception != null)
                sb.Append(Exception.Message);
            
            return sb.ToString();
        }
    }
}