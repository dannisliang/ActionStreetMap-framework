using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders.Roofs
{
    public abstract class RoofBuilder
    {
        protected readonly Model Model;
        protected readonly DynamicMeshGenericMultiMaterialMesh Mesh;
        protected readonly Texture[] Textures;

        protected RoofBuilder(Model model, DynamicMeshGenericMultiMaterialMesh mesh)
        {
            Model = model;
            Mesh = mesh;
            Textures = Model.Textures.ToArray();
        }

        public abstract void Build(Volume volume, Roof design);

        public void AddParapet(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int volumeIndex = area.Volumes.IndexOf(volume);
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up*(numberOfFloors*floorHeight);

            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                int indexA, indexB, indexA0, indexB0;
                Vector3 p0, p1, p00, p10;
                indexA = l;
                indexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                indexA0 = (l > 0) ? l - 1 : numberOfVolumePoints - 1;
                indexB0 = (l < numberOfVolumePoints - 2) ? l + 2 : l + 2 - numberOfVolumePoints;

                int adjacentFloorHeight = area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexA],
                    volume.Points[indexB]);
                bool leftParapet =
                    area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexA0], volume.Points[indexA]) <
                    numberOfFloors;
                bool rightParapet =
                    area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexB], volume.Points[indexB0]) <
                    numberOfFloors;

                if (adjacentFloorHeight >= numberOfFloors)
                    continue; //do not draw a roof edge

                p0 = area.Points[volume.Points[indexA]].Vector3();
                p1 = area.Points[volume.Points[indexB]].Vector3();
                p00 = area.Points[volume.Points[indexA0]].Vector3();
                p10 = area.Points[volume.Points[indexB0]].Vector3();

                float facadeWidth = Vector3.Distance(p0, p1);
                Vector3 facadeDirection = (p1 - p0).normalized;
                Vector3 facadeDirectionLeft = (p0 - p00).normalized;
                Vector3 facadeDirectionRight = (p10 - p1).normalized;
                Vector3 facadeNormal = Vector3.Cross(facadeDirection, Vector3.up);
                Vector3 facadeNormalLeft = Vector3.Cross(facadeDirectionLeft, Vector3.up);
                Vector3 facadeNormalRight = Vector3.Cross(facadeDirectionRight, Vector3.up);

                float parapetHeight = design.ParapetHeight;
                float parapetFrontDepth = design.ParapetFrontDepth;
                float parapetBackDepth = design.ParapetBackDepth;

                Vector3 w0, w1, w2, w3, w4, w5, w6, w7;
                Vector3 pr = facadeDirection*facadeWidth;
                Vector3 pu = Vector3.up*parapetHeight;

                Vector3 pbdl, pbdr, pfdl, pfdr;
                if (leftParapet)
                {
                    pbdl = -(facadeNormal + facadeNormalLeft).normalized*parapetFrontDepth;
                    pfdl = (facadeNormal + facadeNormalLeft).normalized*parapetBackDepth;
                }
                else
                {
                    pbdl = facadeDirectionLeft*parapetFrontDepth;
                    pfdl = -facadeDirectionLeft*parapetBackDepth;
                }
                if (rightParapet)
                {
                    pbdr = -(facadeNormal + facadeNormalRight).normalized*parapetFrontDepth;
                    pfdr = (facadeNormal + facadeNormalRight).normalized*parapetBackDepth;
                }
                else
                {
                    pbdr = -facadeDirectionRight*parapetFrontDepth;
                    pfdr = facadeDirectionRight*parapetBackDepth;
                }

                p0 += volumeFloorHeight;


                w2 = p0 + pbdl; //front left
                w3 = p0 + pr + pbdr; //front right
                w0 = p0 + pfdl; //back left
                w1 = p0 + pr + pfdr; //back right
                w6 = p0 + pbdl + pu; //front left top
                w7 = p0 + pr + pbdr + pu; //front right top
                w4 = p0 + pfdl + pu; //back left top
                w5 = p0 + pr + pfdr + pu; //back right top

                int subMesh = design.GetTexture();
                bool flipped = design.IsFlipped();
                AddPlane(w1, w0, w5, w4, subMesh, flipped); //front
                AddPlaneComplex(w5, w4, w7, w6, subMesh, facadeNormal); //top
                AddPlane(w2, w3, w6, w7, subMesh, flipped); //back

                if (parapetFrontDepth > 0)
                    AddPlaneComplex(w0, w1, w2, w3, subMesh, facadeNormal); //bottom

                if (!leftParapet)
                    AddPlane(w0, w2, w4, w6, subMesh, flipped); //left cap

                if (!rightParapet)
                    AddPlane(w3, w1, w7, w5, subMesh, flipped); //left cap
            }
        }

        protected void Dormers(Volume volume, Roof design)
        {
            Plan area = Model.Plan;
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = Model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up*(numberOfFloors*floorHeight);

            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                int indexA, indexB, indexA0, indexB0;
                Vector3 p0, p1, p00, p10;
                indexA = l;
                indexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                indexA0 = (l > 0) ? l - 1 : numberOfVolumePoints - 1;
                indexB0 = (l < numberOfVolumePoints - 2) ? l + 2 : l + 2 - numberOfVolumePoints;

                p0 = area.Points[volume.Points[indexA]].Vector3();
                p1 = area.Points[volume.Points[indexB]].Vector3();
                p00 = area.Points[volume.Points[indexA0]].Vector3();
                p10 = area.Points[volume.Points[indexB0]].Vector3();

                Vector3 facadeDirection = (p1 - p0).normalized;
                Vector3 facadeDirectionLeft = (p0 - p00).normalized;
                Vector3 facadeDirectionRight = (p10 - p1).normalized;
                Vector3 facadeNormal = Vector3.Cross(p1 - p0, Vector3.up).normalized;
                Vector3 facadeNormalLeft = Vector3.Cross(facadeDirectionLeft, Vector3.up);
                Vector3 facadeNormalRight = Vector3.Cross(facadeDirectionRight, Vector3.up);
                Vector3 leftDir = (facadeNormal + facadeNormalLeft).normalized;
                Vector3 rightDir = (facadeNormal + facadeNormalRight).normalized;

                float windowBottom = (design.Height - design.DormerHeight)*design.DormerHeightRatio;

                float baseDepth = design.FloorDepth;
                float cornerLeftRad = Vector3.Angle(facadeDirection, -facadeDirectionLeft)*Mathf.Deg2Rad/2;
                float cornerRightRad = Vector3.Angle(-facadeDirection, facadeDirectionRight)*Mathf.Deg2Rad/2;
                float cornerDepthLeft = baseDepth/Mathf.Sin(cornerLeftRad);
                float cornerDepthRight = baseDepth/Mathf.Sin(cornerRightRad);
                float topDepth = design.Depth;
                float cornerTopDepthLeft = topDepth/Mathf.Sin(cornerLeftRad);
                float cornerTopDepthRight = topDepth/Mathf.Sin(cornerRightRad);

                float dormerDepth = design.Depth*(design.DormerHeight/design.Height);
                float windowBottomRat = Mathf.Lerp(0, 1 - design.DormerHeight/design.Height, design.DormerHeightRatio);

                p0 += volumeFloorHeight + leftDir*cornerDepthLeft;
                p1 += volumeFloorHeight + rightDir*cornerDepthRight;

                float leftStartTopRad = Vector3.Angle(facadeDirectionLeft, facadeDirection)*Mathf.Deg2Rad*0.5f;
                float leftStartMargin = cornerTopDepthLeft*Mathf.Sin(leftStartTopRad);

                float rightStartTopRad = Vector3.Angle(facadeDirection, facadeDirectionRight)*Mathf.Deg2Rad*0.5f;
                float rightStartMargin = cornerTopDepthRight*Mathf.Sin(rightStartTopRad);

                Vector3 dormerStartPosition = leftDir*(windowBottomRat*cornerTopDepthLeft) +
                                              facadeDirection*(leftStartMargin);
                Vector3 dormerEndPosition = rightDir*(windowBottomRat*cornerTopDepthRight) -
                                            facadeDirection*(rightStartMargin + design.DormerWidth);
                float dormerPositionWidth = Vector3.Distance((p0 + dormerStartPosition), (p1 + dormerEndPosition));
                int numberOfWindows =
                    Mathf.FloorToInt((dormerPositionWidth)/(design.DormerWidth + design.MinimumDormerSpacing));
                float actualWindowSpacing = (dormerPositionWidth - (numberOfWindows*design.DormerWidth))/
                                            (numberOfWindows + 1);
                numberOfWindows++; //add the final window

                Vector3 dormerWidthVector = facadeDirection*design.DormerWidth;
                Vector3 dormerHeightVectorA = Vector3.up*(design.DormerHeight - design.DormerRoofHeight);
                Vector3 dormerHeightVectorB = Vector3.up*design.DormerHeight;
                Vector3 dormerDepthVector = facadeNormal*dormerDepth;
                Vector3 dormerSpace = facadeDirection*(actualWindowSpacing + design.DormerWidth);
                Vector3 dormerSpacer = facadeDirection*(actualWindowSpacing);
                Vector3 dormerYPosition = Vector3.up*windowBottom;

                Vector3 w0, w1, w2, w3, w4, w5, w6, w7, w8, w9;
                for (int i = 0; i < numberOfWindows; i++)
                {
                    w0 = p0 + dormerSpace*(i) + dormerStartPosition + dormerYPosition + dormerSpacer*0.5f;
                    w1 = w0 + dormerWidthVector;
                    w2 = w0 + dormerHeightVectorA;
                    w3 = w1 + dormerHeightVectorA;
                    w4 = w0 + dormerWidthVector/2 + dormerHeightVectorB;

                    w5 = w0 + dormerDepthVector;
                    w6 = w1 + dormerDepthVector;
                    w7 = w2 + dormerDepthVector;
                    w8 = w3 + dormerDepthVector;
                    w9 = w4 + dormerDepthVector;

                    int subMeshwindow = design.GetTexture();
                    int subMeshwall = design.GetTexture();
                    int subMeshtiles = design.GetTexture();
                    bool flippedwall = design.IsFlipped();
                    bool flippedtiles = design.IsFlipped();

                    AddPlane(w1, w6, w3, w8, subMeshwall, flippedwall); //side
                    AddPlane(w5, w0, w7, w2, subMeshwall, flippedwall); //side
                    AddPlane(w3, w8, w4, w9, subMeshtiles, flippedtiles); //roof
                    AddPlane(w7, w2, w9, w4, subMeshtiles, flippedtiles); //roof

                    Vector3[] verts = new Vector3[5] {w0, w1, w2, w3, w4};
                    float roofBottom = (design.DormerHeight - design.DormerRoofHeight)/design.DormerHeight;
                    Vector2[] uvs = new Vector2[5]
                    {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(0, roofBottom),
                        new Vector2(1, roofBottom),
                        new Vector2(0.5f, 1)
                    };
                    int[] tris = new int[9] {1, 0, 2, 1, 2, 3, 2, 4, 3};
                    Mesh.AddData(verts, uvs, tris, subMeshwindow);
                }
            }
        }

        protected void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
        {
            Mesh.AddData(verts, uvs, tris, subMesh);
        }

        protected void AddPlane(Vector3 w0, Vector3 w1, Vector3 w2, Vector3 w3, int subMesh, bool flipped)
        {
            int textureSubmesh = subMesh;
            Texture texture = Textures[textureSubmesh];
            Vector2 uvSize = Vector2.one;
            if (texture.Tiled)
            {
                float planeWidth = Vector3.Distance(w0, w1);
                float planeHeight = Vector3.Distance(w0, w2);
                uvSize = new Vector2(planeWidth*(1.0f/texture.TextureUnitSize.x),
                    planeHeight*(1.0f/texture.TextureUnitSize.y));
                if (texture.Patterned)
                {
                    Vector2 uvunits = texture.TileUnitUV;
                    uvSize.x = Mathf.Ceil(uvSize.x/uvunits.x)*uvunits.x;
                    uvSize.y = Mathf.Ceil(uvSize.y/uvunits.y)*uvunits.y;
                }
            }
            else
            {
                uvSize.x = texture.TiledX;
                uvSize.y = texture.TiledY;
            }
            if (!flipped)
            {
                Mesh.AddPlane(w0, w1, w2, w3, Vector2.zero, uvSize, textureSubmesh);
            }
            else
            {
                uvSize = new Vector2(uvSize.y, uvSize.x);
                Mesh.AddPlane(w0, w1, w2, w3, Vector2.zero, uvSize, textureSubmesh);
            }
        }

        protected void AddPlaneComplex(Vector3 w0, Vector3 w1, Vector3 w2, Vector3 w3, int subMesh, Vector3 facadeNormal)
        {
            Vector3[] verts = new Vector3[4] {w0, w1, w2, w3};
            Vector2[] uvs = BuildingProjectUVs.Project(verts, Vector2.zero, facadeNormal);
            int[] tris = new int[6] {1, 0, 2, 1, 2, 3};

            Mesh.AddData(verts, uvs, tris, subMesh);
        }
    }
}