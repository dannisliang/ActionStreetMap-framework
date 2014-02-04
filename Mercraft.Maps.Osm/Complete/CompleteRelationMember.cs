
namespace Mercraft.Maps.Osm.Complete
{
    /// <summary>
    /// Represents a relationmember.
    /// </summary>
    public class CompleteRelationMember
    {
        /// <summary>
        /// The member.
        /// </summary>
        public CompleteOsmGeo Member { get; set; }

        /// <summary>
        /// The role.
        /// </summary>
        public string Role{ get; set; }
    }
}
