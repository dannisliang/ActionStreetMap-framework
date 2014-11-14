namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class GSV : NmeaMessage
    {
        public class FieldIds
        {
            public static readonly int Sat01Id = 0;
            public static readonly int Sat01Elevation = 1;
            public static readonly int Sat01Azimuth = 2;
            public static readonly int Sat01SNR = 3;

            public static readonly int Sat02Id = 4;
            public static readonly int Sat02Elevation = 5;
            public static readonly int Sat02Azimuth = 6;
            public static readonly int Sat02SNR = 7;

            public static readonly int Sat03Id = 8;
            public static readonly int Sat03Elevation = 9;
            public static readonly int Sat03Azimuth = 10;
            public static readonly int Sat03SNR = 11;

            public static readonly int Sat04Id = 12;
            public static readonly int Sat04Elevation = 13;
            public static readonly int Sat04Azimuth = 14;
            public static readonly int Sat04SNR = 15;

            public static readonly int NumberOfMessages = 16;
            public static readonly int MessageNumber = 17;
            public static readonly int VisibleSatelites = 18;
        }

        public GSV()
        {
            Id = NmeaMessageType.GSV;
            NmeaField f = null;

            for (int i = 0; i < 4; ++i)
            {
                int t = 4*i;

                f = new NmeaField(NmeaValueType.Integer);
                f.Index = new[] {t + 4};
                Fields.Add(t + FieldIds.Sat01Id, f);

                f = new NmeaField(NmeaValueType.Double);
                f.Index = new[] {t + 5};
                Fields.Add(t + FieldIds.Sat01Elevation, f);

                f = new NmeaField(NmeaValueType.Double);
                f.Index = new[] {t + 6};
                Fields.Add(t + FieldIds.Sat01Azimuth, f);

                f = new NmeaField(NmeaValueType.Integer);
                f.Index = new[] {t + 7};
                Fields.Add(t + FieldIds.Sat01SNR, f);
            }

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {1};
            Fields.Add(FieldIds.NumberOfMessages, f);

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {2};
            Fields.Add(FieldIds.MessageNumber, f);

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {3};
            Fields.Add(FieldIds.VisibleSatelites, f);
        }

        public override bool IsApplicable(string[] nmea)
        {
            return nmea[0].Trim().Equals("$GPGSV");
        }

        public override NmeaMessage CreateEmpty()
        {
            return new GSV();
        }
    }
}