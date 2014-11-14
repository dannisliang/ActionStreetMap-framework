using System;

namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class NmeaUtils
    {
        // hhmmss.sss
        public static TimeSpan Str2Time(string s)
        {
            if (s == null)
            {
                return TimeSpan.MinValue;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return TimeSpan.MinValue;
            }
            int h = Convert.ToInt32(s.Substring(0, 2));
            int m = Convert.ToInt32(s.Substring(2, 2));
            int s1 = Convert.ToInt32(s.Substring(4, 2));
            int mls = 0;
            if (s.Length > 6)
            {
                mls = Convert.ToInt32(s.Substring(7, s.Length - 7));
            }
            return new TimeSpan(0, h, m, s1, mls);
        }

        // ddmmyy
        public static DateTime Str2Date(string s)
        {
            if (s == null)
            {
                return DateTime.MinValue;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return DateTime.MinValue;
            }
            int d = Convert.ToInt32(s.Substring(0, 2));
            int m = Convert.ToInt32(s.Substring(2, 2));
            int y = Convert.ToInt32(s.Substring(4, 2));
            // hack
            int t = 2000 + y;
            return new DateTime(t, m, d, 0, 0, 0, 0);
        }

        public static double Str2Degrees(string s, char indicator)
        {
            if (s == null)
            {
                return 0.0;
            }
            double d = Str2Degrees(s);
            return NormalizeDegrees(d, indicator);
        }

        // ddmm.mmmm dddmm.mmmm
        public static double Str2Degrees(string s)
        {
            if (s == null)
            {
                return 0.0;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return 0.0;
            }
            int i = s.IndexOf('.');
            if (i < 0)
            {
                i = s.Length;
            }
            int d = 0;
            if (i > 2)
            {
                d = Convert.ToInt32(s.Substring(0, i - 2));
            }
            i -= 2;
            if (i < 0)
            {
                i = 0;
            }
            double m = Str2Double(s.Substring(i, s.Length - i), 0.0);
            m = d + m/60.0;
            return m;
        }

        public static double NormalizeDegrees(double d, char indicator)
        {
            switch (indicator)
            {
                case 'S':
                    goto case 'W';
                case 'W':
                    d = -d;
                    break;
            }
            return d;
        }

        public static int Str2Int(string s, int defaultVal)
        {
            if (s == null)
            {
                return defaultVal;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return defaultVal;
            }
            return Convert.ToInt32(s);
        }

        public static double Str2Double(string s, double defaultVal)
        {
            if (s == null)
            {
                return defaultVal;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return defaultVal;
            }
            return Convert.ToDouble(s,
                System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public static char Str2Char(string s)
        {
            if (s == null)
            {
                return (char) 0;
            }
            s = s.Trim();
            if (s.Length <= 0)
            {
                return (char) 0;
            }
            return s[0];
        }

        // Speed in knots
        public static double Str2Speed(string s)
        {
            return Str2Speed(s, 'N');
        }

        public static double Str2Speed(string s, char unit)
        {
            if (s == null)
            {
                return -1.0;
            }
            double sp = Str2Double(s, -1.0);
            if (sp >= 0.0)
            {
                if ((unit == (char) 0) || (unit == 'N'))
                {
                    const double knots2Km = 1.852;
                    sp *= knots2Km;
                }
            }
            return sp;
        }
    }
}