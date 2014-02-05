
namespace Mercraft.Maps.Osm.Entities
{
    /// <summary>
    /// Represents simple relation member.
    /// </summary>
    public class RelationMember
    {
        /// <summary>
        /// The type of this relation member.
        /// </summary>
        public ElementType? MemberType { get; set; }


        public Element Member { get; set; }

        /// <summary>
        /// The member id.
        /// </summary>
        public long? MemberId { get; set; }

        /// <summary>
        /// The member role.
        /// </summary>
        public string MemberRole { get; set; }

        /// <summary>
        /// Creates a new relation member.
        /// </summary>
        public static RelationMember Create(int memberId, string memberRole, ElementType memberType)
        {
            RelationMember member = new RelationMember();
            member.MemberId = memberId;
            member.MemberRole = memberRole;
            member.MemberType = memberType;
            return member;
        }
    }
}
