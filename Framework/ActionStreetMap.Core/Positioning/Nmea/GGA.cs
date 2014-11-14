namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class GGA : NmeaMessage
    {
        public class FieldIds
        {
            public static readonly int Utc = 0;
            public static readonly int X = 1; // longitude
            public static readonly int Y = 2; // latitude
            public static readonly int PositionFixIndicator = 3;
            public static readonly int Satelites = 4;
            public static readonly int Hdop = 5;
            public static readonly int Altitude = 6;
            public static readonly int GeoidHeight = 7;
        }

        public GGA()
        {
            Id = NmeaMessageType.GGA;
            NmeaField f = null;

            f = new NmeaField(NmeaValueType.Time);
            f.Index = new[] {1};
            Fields.Add(FieldIds.Utc, f);

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {4, 5};
            Fields.Add(FieldIds.X, f);

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {2, 3};
            Fields.Add(FieldIds.Y, f);

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {6};
            Fields.Add(FieldIds.PositionFixIndicator, f);

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {7};
            Fields.Add(FieldIds.Satelites, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {8};
            Fields.Add(FieldIds.Hdop, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {9};
            Fields.Add(FieldIds.Altitude, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {11};
            Fields.Add(FieldIds.GeoidHeight, f);
        }

        public override bool IsApplicable(string[] nmea)
        {
            return nmea[0].Trim().Equals("$GPGGA");
        }

        public override NmeaMessage CreateEmpty()
        {
            return new GGA();
        }
    }
}