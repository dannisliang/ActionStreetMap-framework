
namespace Mercraft.Maps.Core.Attributes
{
    /// <summary>
    /// Represents an attribute related to a geometry.
    /// </summary>
    public class GeometryAttribute
    {
        /// <summary>
        /// Gets/sets the key property describing the data in this attribute.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets/sets the value of this attribute.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Returns a System.String that represents this GeometryAttribute.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Key != null)
            {
                if (this.Value != null)
                {
                    return string.Format("{0}={1}", this.Key, this.Value.ToString());
                }
                else
                {
                    return string.Format("{0}=null", this.Key);
                }
            }
            else
            {
                if (this.Value != null)
                {
                    return string.Format("null={0}", this.Value.ToString());
                }
                else
                {
                    return "null=null";
                }
            }
        }
    }
}
