using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public class SawtoothRoofBuilder: RoofBuilder
    {
        public SawtoothRoofBuilder(Model model, DynamicMeshGenericMultiMaterialMesh mesh)
            : base(model, mesh)
        {
        }

        public override void Build(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);
            Vector3 ridgeVector = Vector3.up * design.Height;

            int[] pointIndexes = new int[4];
            switch (design.Direction)
            {
                case 0:
                    pointIndexes = new int[4] { 0, 1, 2, 3 };
                    break;
                case 1:
                    pointIndexes = new int[4] { 1, 2, 3, 0 };
                    break;
                case 2:
                    pointIndexes = new int[4] { 2, 3, 0, 1 };
                    break;
                case 3:
                    pointIndexes = new int[4] { 3, 0, 1, 2 };
                    break;
            }
            Vector3[] basepoints = new Vector3[4];
            Vector3[] points = new Vector3[6];

            for (int i = 0; i < design.SawtoothTeeth; i++)
            {

                Vector3 toothBaseMovementA = (area.Points[volume.Points[pointIndexes[3]]].Vector3() - area.Points[volume.Points[pointIndexes[0]]].Vector3()).normalized;
                float roofDepthA = Vector3.Distance(area.Points[volume.Points[pointIndexes[3]]].Vector3(), area.Points[volume.Points[pointIndexes[0]]].Vector3());
                float toothDepthA = roofDepthA / design.SawtoothTeeth;
                Vector3 toothVectorA = toothBaseMovementA * toothDepthA;

                Vector3 toothBaseMovementB = (area.Points[volume.Points[pointIndexes[2]]].Vector3() - area.Points[volume.Points[pointIndexes[1]]].Vector3()).normalized;
                float roofDepthB = Vector3.Distance(area.Points[volume.Points[pointIndexes[2]]].Vector3(), area.Points[volume.Points[pointIndexes[1]]].Vector3());
                float toothDepthB = roofDepthB / design.SawtoothTeeth;
                Vector3 toothVectorB = toothBaseMovementB * toothDepthB;

                basepoints[0] = area.Points[volume.Points[pointIndexes[0]]].Vector3() + toothVectorA * i;
                basepoints[1] = area.Points[volume.Points[pointIndexes[1]]].Vector3() + toothVectorB * i;
                basepoints[2] = basepoints[1] + toothVectorB;
                basepoints[3] = basepoints[0] + toothVectorA;

                points[0] = basepoints[0] + volumeFloorHeight;
                points[1] = basepoints[1] + volumeFloorHeight;
                points[2] = basepoints[2] + volumeFloorHeight;
                points[3] = basepoints[3] + volumeFloorHeight;
                points[4] = basepoints[2] + volumeFloorHeight + ridgeVector;
                points[5] = basepoints[3] + volumeFloorHeight + ridgeVector;

                //top
                int subMeshTop = design.GetTexture();
                bool flippedTop = design.IsFlipped();
                AddPlane(points[0], points[1], points[5], points[4], subMeshTop, flippedTop);

                //window
                int subMeshWindow = design.GetTexture();
                bool flippedWindow = design.IsFlipped();
                AddPlane(points[2], points[3], points[4], points[5], subMeshWindow, flippedWindow);

                //sides
                Vector3[] vertsA = new Vector3[3] { points[1], points[2], points[4] };
                Vector3[] vertsB = new Vector3[3] { points[0], points[3], points[5] };
                float uvWdith = Vector3.Distance(points[0], points[3]);
                float uvHeight = design.Height;
                int subMesh = design.GetTexture();
                Texture texture = Textures[subMesh];

                if (texture.Tiled)
                {
                    uvWdith *= (1.0f / texture.TextureUnitSize.x);
                    uvHeight *= (1.0f / texture.TextureUnitSize.y);
                    if (texture.Patterned)
                    {
                        Vector2 uvunits = texture.TileUnitUV;
                        uvWdith = Mathf.Ceil(uvWdith / uvunits.x) * uvunits.x;
                        uvHeight = Mathf.Ceil(uvHeight / uvunits.y) * uvunits.y;
                    }
                }
                else
                {
                    uvWdith = texture.TiledX;
                    uvHeight = texture.TiledY;
                }

                Vector2[] uvs = new Vector2[3] { new Vector2(0, 0), new Vector2(uvWdith, 0), new Vector2(uvWdith, uvHeight) };
                int[] triA = new int[3] { 1, 0, 2 };
                int[] triB = new int[3] { 0, 1, 2 };
                AddData(vertsA, uvs, triA, subMesh);
                AddData(vertsB, uvs, triB, subMesh);

            }
        }
    }
}