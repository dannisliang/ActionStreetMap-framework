using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class NmeaParser: IDisposable
    {
        private readonly Stream _sourceStream;
        private readonly List<NmeaMessage> _handlers = new List<NmeaMessage>(6)
        {
            new GGA(),
            new RMC(),
            new GLL(),
            new GSA(),
            new GSV()
        };

        public NmeaParser(Stream sourceStream)
        {
            _sourceStream = sourceStream;
        }    

        public NmeaMessage ParseLine(string line)
        {
            if ((line == null) || (line.Length <= 0))
                return null;
            
            NmeaMessage outMessage = null;
            try
            {
                int csIndex = line.LastIndexOf('*');
                if (csIndex <= 0)
                    return null;
                
                csIndex++;
                if (csIndex >= line.Length)
                    return null;
                
                string cs = line.Substring(csIndex, line.Length - csIndex);
                string tempLine = line.Substring(0, csIndex - 1);
                string[] parts = tempLine.ToUpper().Split(',');
                if (parts.Length <= 0)
                    return null;
                
                if (!ValidateChecksum(tempLine, cs))
                    return null;

                foreach (NmeaMessage message in _handlers)
                {
                    if (message.IsApplicable(parts))
                    {
                        outMessage = message.CreateEmpty();
                        outMessage.FromNmea(parts);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                outMessage = new Error(line, ex);
            }

            return outMessage;
        }

        private bool ValidateChecksum(string line, string cs)
        {
            int crc = Convert.ToInt32(cs, 16);
            int checksum = 0;
            for (int i = 1; i < line.Length; i++)
            {
                checksum ^= Convert.ToByte(line[i]);
            }
            return (checksum == crc);
        }

        public string ReadLine()
        {
            var line = new StringBuilder();
            while (true)
            {
                int c = _sourceStream.ReadByte();
                if (c < 0)
                {
                    if (line.Length <= 0)
                        return null;
                    break;
                }
                // LF and CR
                if ((c == 10) || (c == 13))
                {
                    if (line.Length <= 0)
                        continue;
                    break;
                }
                line.Append((char) c);
            }
            return line.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                _sourceStream.Dispose();
            }
        }
    }
}