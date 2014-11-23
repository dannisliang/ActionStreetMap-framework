namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class RMC : NmeaMessage
    {
        public class FieldIds
        {
            public static readonly int Utc = 0;
            public static readonly int Status = 1;
            public static readonly int X = 2; // longitude
            public static readonly int Y = 3; // latitude
            public static readonly int Speed = 4;
            public static readonly int Course = 5;
            public static readonly int Date = 6;
        }

        public RMC()
        {
            Id = NmeaMessageType.RMC;
            NmeaField f = null;

            f = new NmeaField(NmeaValueType.Time);
            f.Index = new[] {1};
            Fields.Add(FieldIds.Utc, f);

            f = new NmeaField(NmeaValueType.Enum);
            f.Index = new[] {2};
            Fields.Add(FieldIds.Status, f);

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {3, 4};
            Fields.Add(FieldIds.Y, f);

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {5, 6};
            Fields.Add(FieldIds.X, f);

            f = new NmeaField(NmeaValueType.Speed);
            f.Index = new[] {7};
            Fields.Add(FieldIds.Speed, f);

            f = new NmeaField(NmeaValueType.Degrees);
            f.Index = new[] {8};
            Fields.Add(FieldIds.Course, f);

            f = new NmeaField(NmeaValueType.Date);
            f.Index = new[] {9};
            Fields.Add(FieldIds.Date, f);
        }

        public override bool IsApplicable(string[] nmea)
        {
            return nmea[0].Trim().Equals("$GPRMC");
        }

        public override NmeaMessage CreateEmpty()
        {
            return new RMC();
        }
    }
}