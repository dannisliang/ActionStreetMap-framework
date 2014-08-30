using System.Linq;
using Mercraft.Core;
using Mercraft.Core.World.Buildings;
using UnityEngine;

namespace Mercraft.Models.Buildings.Facades
{
    public class FlatFacadeBuilder: IFacadeBuilder
    {
        public string Name { get{ return "flat"; } }

        public MeshData Build(Building building, BuildingStyle style)
        {
            var vertices2D = building.Footprint;

            return new MeshData()
            {
                Vertices = GetVerticies3D(vertices2D, building.Height),
                Triangles = GetTriangles3D(vertices2D),
                UV = GetUV(vertices2D),
                TextureKey = style.Roof.Texture,
                MaterialKey = style.Roof.Material
            };
        }

        private Vector3[] GetVerticies3D(MapPoint[] maPoints, float height)
        {
            var length = maPoints.Length;
            var verticies3D = new Vector3[length * 4];

            for (int i = 0; i < length; i++)
            {
                var v3DIndex = i * 4;
                var v2DIndex = i == (length - 1) ? 0 : i + 1;

                verticies3D[v3DIndex] = new Vector3(maPoints[i].X, maPoints[i].Elevation, maPoints[i].Y);

                verticies3D[v3DIndex + 1] = new Vector3(maPoints[v2DIndex].X, maPoints[v2DIndex].Elevation, maPoints[v2DIndex].Y);
                verticies3D[v3DIndex + 2] = new Vector3(maPoints[v2DIndex].X, maPoints[v2DIndex].Elevation + height, maPoints[v2DIndex].Y);

                verticies3D[v3DIndex + 3] = new Vector3(maPoints[i].X, maPoints[i].Elevation + height, maPoints[i].Y);
            }

            return verticies3D;
        }

        private int[] GetTriangles3D(MapPoint[] verticies2D)
        {
            var length = verticies2D.Count();
            var triangles = new int[(length) * 2 * 3];
            for (int i = 0; i < length; i++)
            {
                var tIndex = i * 6;
                var vIndex = i * 4;
                triangles[tIndex] = vIndex;
                triangles[tIndex + 1] = vIndex + 1;
                triangles[tIndex + 2] = vIndex + 2;

                triangles[tIndex + 3] = vIndex + 3;
                triangles[tIndex + 4] = vIndex + 0;
                triangles[tIndex + 5] = vIndex + 2;
            }

            return triangles;
        }

        private Vector2[] GetUV(MapPoint[] verticies2D)
        {
            var length = verticies2D.Length;
            var uv = new Vector2[(length) * 4];

            for (int i = 0; i < length; i++)
            {
                var vIndex = i * 4;
                uv[vIndex] = new Vector2(1, 0);
                uv[vIndex + 1] = new Vector2(0, 0);
                uv[vIndex + 2] = new Vector2(0, 1);
                uv[vIndex + 3] = new Vector2(1, 1);
            }

            return uv;
        }
    }
}
