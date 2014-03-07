
namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents simple relation member.
    /// </summary>
    public class RelationMember
    {
        /// <summary>
        /// Member
        /// </summary>
        public Element Member { get; set; }

        /// <summary>
        /// The member id.
        /// </summary>
        public long MemberId { get; set; }

        /// <summary>
        /// The member role.
        /// </summary>
        public string MemberRole { get; set; }

    }
}
