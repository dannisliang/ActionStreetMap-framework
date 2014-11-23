using System;
using System.Text;

namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class NmeaField
    {
        public int[] Index = null;
        private readonly NmeaValueType _nmeaValueType = NmeaValueType.Integer;
        private object _value;

        public ActualNmeaValueType ActualNmeaType
        {
            get { return Value2ActualType(_nmeaValueType); }
        }

        public NmeaField()
        {
        }

        public NmeaField(NmeaValueType nmeaValueType)
        {
            _nmeaValueType = nmeaValueType;
        }

        public bool HasValue
        {
            get { return (_value != null); }
        }

        private string GetIndexedString(string[] nmea, int indexVal)
        {
            if (indexVal >= Index.Length)
            {
                return null;
            }
            int i = Index[indexVal];
            if ((i < 0) || (i >= nmea.Length))
            {
                return null;
            }
            return nmea[i];
        }

        public void Parse(string[] nmea)
        {
            _value = null;
            if ((Index == null) || (Index.Length <= 0))
            {
                return;
            }
            switch (_nmeaValueType)
            {
                case NmeaValueType.Integer:
                    _value = NmeaUtils.Str2Int(GetIndexedString(nmea, 0), 0);
                    break;
                case NmeaValueType.Double:
                    _value = NmeaUtils.Str2Double(GetIndexedString(nmea, 0), 0);
                    break;
                case NmeaValueType.Time:
                    _value = NmeaUtils.Str2Time(GetIndexedString(nmea, 0));
                    break;
                case NmeaValueType.GeoDegrees:
                    _value = NmeaUtils.Str2Degrees(
                        GetIndexedString(nmea, 0),
                        NmeaUtils.Str2Char(GetIndexedString(nmea, 1)));
                    break;
                case NmeaValueType.Enum:
                    _value = NmeaUtils.Str2Char(GetIndexedString(nmea, 0));
                    break;
                case NmeaValueType.Degrees:
                    _value = NmeaUtils.Str2Double(GetIndexedString(nmea, 0), -1.0);
                    break;
                case NmeaValueType.Date:
                    _value = NmeaUtils.Str2Date(GetIndexedString(nmea, 0));
                    break;
                case NmeaValueType.Speed:
                    _value = NmeaUtils.Str2Speed(GetIndexedString(nmea, 0),
                        NmeaUtils.Str2Char(GetIndexedString(nmea, 1)));
                    break;
            }
        }

        public double GetDouble(double defaultVal)
        {
            if (!HasValue || (ActualNmeaType != ActualNmeaValueType.Double)) return defaultVal;
            double d = (double) _value;
            return d;
        }

        public int GetInt(int defaultVal)
        {
            if (!HasValue || (ActualNmeaType != ActualNmeaValueType.Integer)) return defaultVal;
            int d = (int) _value;
            return d;
        }

        public char GetChar(char defaultVal)
        {
            if (!HasValue || (ActualNmeaType != ActualNmeaValueType.Char)) return defaultVal;
            char d = (char) _value;
            return d;
        }

        public TimeSpan GetTime(TimeSpan defaultVal)
        {
            if (!HasValue || (ActualNmeaType != ActualNmeaValueType.Time)) return defaultVal;
            TimeSpan d = (TimeSpan) _value;
            return d;
        }

        public DateTime GetDate(DateTime defaultVal)
        {
            if (!HasValue || (ActualNmeaType != ActualNmeaValueType.Date)) return defaultVal;
            DateTime d = (DateTime) _value;
            return d;
        }

        private static ActualNmeaValueType Value2ActualType(NmeaValueType t)
        {
            switch (t)
            {
                case NmeaValueType.Double:
                    return ActualNmeaValueType.Double;
                case NmeaValueType.Time:
                    return ActualNmeaValueType.Time;
                case NmeaValueType.Integer:
                    return ActualNmeaValueType.Integer;
                case NmeaValueType.GeoDegrees:
                    return ActualNmeaValueType.Double;
                case NmeaValueType.Enum:
                    return ActualNmeaValueType.Char;
                case NmeaValueType.Speed:
                    return ActualNmeaValueType.Double;
                case NmeaValueType.Degrees:
                    return ActualNmeaValueType.Double;
                case NmeaValueType.Date:
                    return ActualNmeaValueType.Date;
            }
            return ActualNmeaValueType.Other;
        }

        // for debug only
        public override string ToString()
        {
            //TODO
            var sb = new StringBuilder();
            sb.Append("IDX: ").Append(Index[0]).Append(" T:").Append(_nmeaValueType).Append(" V:");
            if (_value == null)
                sb.Append("NULL");

            return sb.ToString();
        }
    }
}