namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    ///     Represents simple relation member.
    /// </summary>
    public class RelationMember
    {
        /// <summary>
        ///     Member.
        /// </summary>
        public Element Member { get; set; }

        /// <summary>
        ///     Member id.
        /// </summary>
        public long MemberId { get; set; }

        /// <summary>
        ///     Member role.
        /// </summary>
        public string Role { get; set; }
    }
}