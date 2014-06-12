using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class SteepledRoofBuilder : RoofBuilder
    {
        public SteepledRoofBuilder(Model model, DynamicMultiMaterialMesh mesh)
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

            int numberOfVolumePoints = volume.Points.Count;
            Vector3[] basePoints = new Vector3[numberOfVolumePoints];
            Vector3 centrePoint = Vector3.zero;
            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                basePoints[l] = area.Points[volume.Points[l]].Vector3() + volumeFloorHeight;
                centrePoint += area.Points[volume.Points[l]].Vector3();
            }
            centrePoint = (centrePoint/numberOfVolumePoints) + volumeFloorHeight + ridgeVector;
            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                int pointIndexA = l;
                int pointIndexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                Vector3[] verts = new Vector3[3] {basePoints[pointIndexA], basePoints[pointIndexB], centrePoint};
                float uvWdith = Vector3.Distance(basePoints[pointIndexA], basePoints[pointIndexB]);
                float uvHeight = design.Height;
                int subMesh = design.GetTexture();
                Texture texture = Textures[subMesh];

                if (texture.Tiled)
                {
                    uvWdith *= (1.0f/texture.TextureUnitSize.x);
                    uvHeight *= (1.0f/texture.TextureUnitSize.y);
                    if (texture.Patterned)
                    {
                        Vector2 uvunits = texture.TileUnitUV;
                        uvWdith = Mathf.Ceil(uvWdith/uvunits.x)*uvunits.x;
                        uvHeight = Mathf.Ceil(uvHeight/uvunits.y)*uvunits.y;
                    }
                }
                else
                {
                    uvWdith = texture.TiledX;
                    uvHeight = texture.TiledY;
                }
                Vector2[] uvs = new Vector2[3]
                {new Vector2(-uvWdith/2, 0), new Vector2(uvWdith/2, 0), new Vector2(0, uvHeight)};
                int[] tri = new int[3] {1, 0, 2};
                AddData(verts, uvs, tri, subMesh);
            }
        }
    }
}