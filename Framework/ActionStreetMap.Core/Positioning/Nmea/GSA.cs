namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class GSA : NmeaMessage
    {
        public class FieldIds
        {
            public static readonly int SateliteCh01 = 0;
            public static readonly int SateliteCh02 = 1;
            public static readonly int SateliteCh03 = 2;
            public static readonly int SateliteCh04 = 3;
            public static readonly int SateliteCh05 = 4;
            public static readonly int SateliteCh06 = 5;
            public static readonly int SateliteCh07 = 6;
            public static readonly int SateliteCh08 = 7;
            public static readonly int SateliteCh09 = 8;
            public static readonly int SateliteCh10 = 9;
            public static readonly int SateliteCh11 = 10;
            public static readonly int SateliteCh12 = 11;

            public static readonly int Mode1 = 12;
            public static readonly int Mode2 = 13;
            public static readonly int Pdop = 14;
            public static readonly int Hdop = 15;
            public static readonly int Vdop = 16;
        }

        public GSA()
        {
            Id = NmeaMessageType.GSA;
            NmeaField f = null;

            for (int i = 0; i < 12; ++i)
            {
                f = new NmeaField(NmeaValueType.Integer);
                f.Index = new[] {3 + i};
                Fields.Add(i, f);
            }

            f = new NmeaField(NmeaValueType.Enum);
            f.Index = new[] {1};
            Fields.Add(FieldIds.Mode1, f);

            f = new NmeaField(NmeaValueType.Integer);
            f.Index = new[] {2};
            Fields.Add(FieldIds.Mode2, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {15};
            Fields.Add(FieldIds.Pdop, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {16};
            Fields.Add(FieldIds.Hdop, f);

            f = new NmeaField(NmeaValueType.Double);
            f.Index = new[] {17};
            Fields.Add(FieldIds.Vdop, f);
        }

        public override bool IsApplicable(string[] nmea)
        {
            return nmea[0].Trim().Equals("$GPGSA");
        }

        public override NmeaMessage CreateEmpty()
        {
            return new GSA();
        }
    }
}