using Mercraft.Core.Algorithms;
using Mercraft.Core.Elevation;
using Mercraft.Core.Unity;

namespace Mercraft.Core.Scene.Models
{
    /// <summary>
    ///     Represents map tile
    /// </summary>
    public class Tile : Model
    {
        /// <summary>
        ///     Stores map center coordinate in lat/lon
        /// </summary>
        public GeoCoordinate RelativeNullPoint { get; private set; }

        /// <summary>
        ///     Stores tile center coordinate in Unity metrics
        /// </summary>
        public MapPoint MapCenter { get; private set; }

        /// <summary>
        ///     Gets bounding box for current tile
        /// </summary>
        public BoundingBox BoundingBox { get; private set; }

        /// <summary>
        ///     Square side size in Unity metrics
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        ///     Gets or sets game object which is used to represent this tile
        /// </summary>
        public IGameObject GameObject { get; set; }

        /// <summary>
        ///     Gets or sets heightmap of given tile
        /// </summary>
        public HeightMap HeightMap { get; set; }

        public MapPoint TopLeft { get; set; }
        public MapPoint TopRight { get; set; }
        public MapPoint BottomLeft { get; set; }
        public MapPoint BottomRight { get; set; }

        public Tile(GeoCoordinate relativeNullPoint, MapPoint mapCenter, float size)
        {
            RelativeNullPoint = relativeNullPoint;
            MapCenter = mapCenter;
            Size = size;

            var geoCenter = GeoProjection.ToGeoCoordinate(relativeNullPoint, mapCenter);
            BoundingBox = BoundingBox.CreateBoundingBox(geoCenter, size / 2);

            TopLeft = new MapPoint(MapCenter.X - Size/2, MapCenter.Y + Size/2);
            BottomRight = new MapPoint(MapCenter.X + Size / 2, MapCenter.Y - Size / 2);

            TopRight = new MapPoint(MapCenter.X + Size / 2, MapCenter.Y + Size / 2);
            BottomLeft = new MapPoint(MapCenter.X - Size / 2, MapCenter.Y - Size / 2);
        }

        /// <summary>
        ///     Checks whether absolute position locates in tile with bound offset
        /// </summary>
        /// <param name="position">Absolute position in game</param>
        /// <param name="offset">offset from bounds</param>
        public bool Contains(MapPoint position, float offset)
        {
            return (position.X > TopLeft.X + offset) && (position.Y < TopLeft.Y - offset) &&
                         (position.X < BottomRight.X - offset) && (position.Y > BottomRight.Y + offset);
        }

        public override bool IsClosed
        {
            get { throw new System.NotImplementedException(); }
        }

        public override void Accept(IModelVisitor visitor)
        {
            visitor.VisitTile(this);
        }
    }
}