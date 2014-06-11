using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class GabledRoofBuilder : RoofBuilder
    {
        public GabledRoofBuilder(Model model, DynamicMeshGenericMultiMaterialMesh mesh)
            : base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up*(numberOfFloors*floorHeight);
            Vector3 ridgeVector = Vector3.up*design.Height;

            Vector3[] basePoints = new Vector3[4];
            if (design.Direction == 0)
            {
                basePoints[0] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
                basePoints[1] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                basePoints[2] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
                basePoints[3] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
            }
            else
            {
                basePoints[0] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                basePoints[1] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
                basePoints[2] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
                basePoints[3] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
            }
            Vector3 centrePoint = Vector3.zero;
            for (int l = 0; l < 4; l++)
                centrePoint += area.Points[volume.Points[l]].Vector3();

            Vector3 r0 = Vector3.Lerp(basePoints[0], basePoints[1], 0.5f) + ridgeVector;
            Vector3 r1 = Vector3.Lerp(basePoints[2], basePoints[3], 0.5f) + ridgeVector;

            int subMesh = design.GetTexture();
            bool flipped = design.IsFlipped();
            AddPlane(basePoints[0], r0, basePoints[3], r1, subMesh, flipped); //top
            AddPlane(basePoints[2], r1, basePoints[1], r0, subMesh, flipped); //top

            Vector3[] vertsA = new Vector3[3] {basePoints[0], basePoints[1], r0};
            Vector3[] vertsB = new Vector3[3] {basePoints[2], basePoints[3], r1};
            float uvWdithA = Vector3.Distance(basePoints[0], basePoints[1]);
            float uvWdithB = Vector3.Distance(basePoints[2], basePoints[3]);
            float uvHeight = design.Height;
            subMesh = design.GetTexture();
            Texture texture = Textures[subMesh];

            if (texture.Tiled)
            {
                uvWdithA *= (1.0f/texture.TextureUnitSize.x);
                uvWdithB *= (1.0f/texture.TextureUnitSize.x);
                uvHeight *= (1.0f/texture.TextureUnitSize.y);
                if (texture.Patterned)
                {
                    Vector2 uvunits = texture.TileUnitUV;
                    uvWdithA = Mathf.Ceil(uvWdithA/uvunits.x)*uvunits.x;
                    uvWdithB = Mathf.Ceil(uvWdithB/uvunits.x)*uvunits.x;
                    uvHeight = Mathf.Ceil(uvHeight/uvunits.y)*uvunits.y;
                }
            }
            else
            {
                uvWdithA = texture.TiledX;
                uvWdithB = texture.TiledX;
                uvHeight = texture.TiledY;
            }
            Vector2[] uvsA = new Vector2[3]
            {new Vector2(-uvWdithA/2, 0), new Vector2(uvWdithA/2, 0), new Vector2(0, uvHeight)};
            Vector2[] uvsB = new Vector2[3]
            {new Vector2(-uvWdithB/2, 0), new Vector2(uvWdithB/2, 0), new Vector2(0, uvHeight)};
            int[] tri = new int[3] {1, 0, 2};
            AddData(vertsA, uvsA, tri, subMesh);
            AddData(vertsB, uvsB, tri, subMesh);
        }
    }
}