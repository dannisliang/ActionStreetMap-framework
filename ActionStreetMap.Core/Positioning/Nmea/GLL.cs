namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class GLL : NmeaMessage
    {
        public class FieldIds
        {
            public static readonly int Utc = 0;
            public static readonly int Status = 1;
            public static readonly int X = 2; // longitude
            public static readonly int Y = 3; // latitude
        }

        public GLL()
        {
            Id = NmeaMessageType.GLL;
            NmeaField f = null;

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {1, 2};
            Fields.Add(FieldIds.Y, f);

            f = new NmeaField(NmeaValueType.GeoDegrees);
            f.Index = new[] {3, 4};
            Fields.Add(FieldIds.X, f);

            f = new NmeaField(NmeaValueType.Time);
            f.Index = new[] {5};
            Fields.Add(FieldIds.Utc, f);

            f = new NmeaField(NmeaValueType.Enum);
            f.Index = new[] {6};
            Fields.Add(FieldIds.Status, f);
        }

        public override bool IsApplicable(string[] nmea)
        {
            return nmea[0].Trim().Equals("$GPGLL");
        }

        public override NmeaMessage CreateEmpty()
        {
            return new GLL();
        }
    }
}