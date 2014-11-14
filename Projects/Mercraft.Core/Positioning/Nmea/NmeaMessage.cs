using System.Collections;
using System.Text;

namespace ActionStreetMap.Core.Positioning.Nmea
{
    internal class NmeaMessage
    {
        public NmeaMessageType Id { get; set; }

        public NmeaMessage()
        {
            Id = NmeaMessageType.Error;
            Fields = new Hashtable();
        }

        public NmeaMessage(NmeaMessageType id) : this()
        {
            Id = id;
        }

        internal Hashtable Fields { get; set; }

        /// <summary>
        ///     True if can parse a given nmea
        /// </summary>
        /// <param name="nmea"></param>
        public virtual bool IsApplicable(string[] nmea)
        {
            return false;
        }

        public virtual NmeaMessage CreateEmpty()
        {
            return new NmeaMessage();
        }

        public virtual void FromNmea(string[] p)
        {
            foreach (int i in Fields.Keys)
            {
                NmeaField f = (NmeaField) Fields[i];
                if (f == null) continue;
                f.Parse(p);
            }
        }

        // for debug only
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id).Append(" ");
            foreach (int i in Fields.Keys)
            {
                NmeaField f = (NmeaField) Fields[i];
                if (f == null) continue;
                sb.Append(f);
                sb.Append(" ~ ");
            }
            return sb.ToString();
        }
    }
}