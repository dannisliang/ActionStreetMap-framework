using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class RoofBuilder
    {
        private readonly Model _model;
        private readonly DynamicMeshGenericMultiMaterialMesh _mesh;
        private readonly Texture[] _textures;

        public RoofBuilder(Model model, DynamicMeshGenericMultiMaterialMesh mesh)
        {
            _model = model;
            _mesh = mesh;
            _textures = _model.Textures.ToArray();
        }

        public void Build()
        {
            Build(false);
        }

        public void Build(bool ignoreParapets)
        {

            Plan plan = _model.Plan;

            int numberOfVolumes = _model.Plan.Volumes.Count;
            for (int s = 0; s < numberOfVolumes; s++)
            {

                Volume volume = plan.Volumes[s];
                Roof design = _model.Roofs[volume.RoofDesignId];

                var roofStyle = design.Style;
                if (volume.Points.Count != 4)
                    if (design.Style == RoofStyle.Leanto || design.Style == RoofStyle.Sawtooth || design.Style == RoofStyle.Barrel)
                        roofStyle = RoofStyle.Flat;

                if (volume.Points.Count != 4 && design.Style == RoofStyle.Gabled)
                    roofStyle = RoofStyle.Hipped;//ignore style and just do a flat roof

                switch (roofStyle)
                {
                    case RoofStyle.Flat:
                        FlatRoof(volume, design);
                        break;
                    case RoofStyle.Mansard:
                        Mansard(volume, design);
                        if (design.HasDormers)
                            Dormers(volume, design);
                        break;
                    case RoofStyle.Gabled:
                        Gabled(volume, design);
                        break;
                    case RoofStyle.Hipped:
                        Hipped(volume, design);
                        break;
                    case RoofStyle.Leanto:
                        LeanTo(volume, design);
                        break;
                    case RoofStyle.Sawtooth:
                        Sawtooth(volume, design);
                        break;
                    case RoofStyle.Barrel:
                        Barrel(volume, design);
                        break;
                    case RoofStyle.Steepled:
                        Steeple(volume, design);
                        break;
                }

                if (design.Parapet && !ignoreParapets)
                    Parapet(volume, design);

            }
        }

        private void FlatRoof(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int volumeIndex = area.Volumes.IndexOf(volume);
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

            //add top base of the flat roof
            Vector3[] newEndVerts = new Vector3[numberOfVolumePoints];
            Vector2[] newEndUVs = new Vector2[numberOfVolumePoints];
            int[] tris = new List<int>(area.GetTrianglesBySectorBase(volumeIndex)).ToArray();
            int roofTextureID = design.GetTexture();
            Texture texture = _model.Textures[roofTextureID];
            for (int i = 0; i < numberOfVolumePoints; i++)
            {
                Vector2 point = area.Points[volume.Points[i]];
                newEndVerts[i] = point.Vector3() + volumeFloorHeight;
                newEndUVs[i] = new Vector2(point.x / texture.TextureUnitSize.x, point.y / texture.TextureUnitSize.y);
            }

            AddData(newEndVerts, newEndUVs, tris, design.GetTexture());
        }

        private void Mansard(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

            //add top base of the flat roof
            Vector3[] topVerts = new Vector3[numberOfVolumePoints];
            Vector2[] topUVs = new Vector2[numberOfVolumePoints];
            int topTextureID = design.GetTexture();
            Texture texture = _textures[topTextureID];

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

                float facadeWidth = Vector3.Distance(p0, p1);
                Vector3 facadeDirection = (p1 - p0).normalized;
                Vector3 facadeDirectionLeft = (p0 - p00).normalized;
                Vector3 facadeDirectionRight = (p10 - p1).normalized;
                Vector3 facadeNormal = Vector3.Cross(facadeDirection, Vector3.up);
                Vector3 facadeNormalLeft = Vector3.Cross(facadeDirectionLeft, Vector3.up);
                Vector3 facadeNormalRight = Vector3.Cross(facadeDirectionRight, Vector3.up);

                float roofHeight = design.Height;
                float baseDepth = design.FloorDepth;
                float cornerLeftRad = Vector3.Angle(facadeDirection, -facadeDirectionLeft) * Mathf.Deg2Rad / 2;
                float cornerRightRad = Vector3.Angle(-facadeDirection, facadeDirectionRight) * Mathf.Deg2Rad / 2;
                float cornerDepthLeft = baseDepth / Mathf.Sin(cornerLeftRad);
                float cornerDepthRight = baseDepth / Mathf.Sin(cornerRightRad);
                float topDepth = design.Depth;
                float cornerTopDepthLeft = topDepth / Mathf.Sin(cornerLeftRad);
                float cornerTopDepthRight = topDepth / Mathf.Sin(cornerRightRad);

                Vector3 pr = facadeDirection * facadeWidth;

                Vector3 leftDir = (facadeNormal + facadeNormalLeft).normalized;
                Vector3 rightDir = (facadeNormal + facadeNormalRight).normalized;

                p0 += volumeFloorHeight;
                

                Vector3 w0, w1, w2, w3, w4, w5;
                w0 = p0;
                w1 = p0 + pr;
                w2 = w0 + leftDir * cornerDepthLeft;
                w3 = w1 + rightDir * cornerDepthRight;
                w4 = w2 + leftDir * cornerTopDepthLeft + Vector3.up * roofHeight;
                w5 = w3 + rightDir * cornerTopDepthRight + Vector3.up * roofHeight;

                Vector3[] verts = new Vector3[6] { w0, w1, w2, w3, w4, w5 };
                List<Vector2> uvs = new List<Vector2>();

                Vector2[] uvsFloor = BuildingProjectUVs.Project(new Vector3[4] { w0, w1, w2, w3 }, Vector2.zero, facadeNormal);
                Vector2[] uvsMansard = BuildingProjectUVs.Project(new Vector3[3] { w2, w4, w5 }, uvsFloor[2], facadeNormal);
                uvs.AddRange(uvsFloor);
                uvs.Add(uvsMansard[1]);
                uvs.Add(uvsMansard[2]);

                Vector3[] vertsA = new Vector3[4] { verts[0], verts[1], verts[2], verts[3] };
                Vector2[] uvsA = new Vector2[4] { uvsFloor[0], uvsFloor[1], uvsFloor[2], uvsFloor[3] };
                int[] trisA = new int[6] { 1, 0, 2, 1, 2, 3 };
                int subMeshA = design.GetTexture();
                _mesh.AddData(vertsA, uvsA, trisA, subMeshA);

                Vector3[] vertsB = new Vector3[4] { verts[2], verts[3], verts[4], verts[5] };
                Vector2[] uvsB = new Vector2[4] { uvsFloor[2], uvsFloor[3], uvsMansard[1], uvsMansard[2] };
                int[] trisB = new int[6] { 0, 2, 1, 1, 2, 3 };
                int subMeshB = design.GetTexture();
                _mesh.AddData(vertsB, uvsB, trisB, subMeshB);

                //modify point for the top geometry
                Vector2 point = area.Points[volume.Points[l]];
                topVerts[l] = point.Vector3() + volumeFloorHeight + Vector3.up * roofHeight + leftDir * (cornerDepthLeft + cornerTopDepthLeft);
                topUVs[l] = new Vector2(topVerts[l].x / texture.TextureUnitSize.x, topVerts[l].z / texture.TextureUnitSize.y);
            }

            Vector2[] topVertV2z = new Vector2[topVerts.Length];
            for (int i = 0; i < topVerts.Length; i++)
                topVertV2z[i] = new Vector2(topVerts[i].x, topVerts[i].z);
            int[] topTris = EarClipper.Triangulate(topVertV2z);
            AddData(topVerts, topUVs, topTris, topTextureID);//top
        }

        private void Barrel(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

            Vector3[] points = new Vector3[4];
            //Vector3 ridgeVector;
            if (design.Direction == 0)
            {
                points[0] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
                points[1] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
                points[2] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                points[3] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
            }
            else
            {
                points[0] = area.Points[volume.Points[0]].Vector3() + volumeFloorHeight;
                points[1] = area.Points[volume.Points[1]].Vector3() + volumeFloorHeight;
                points[2] = area.Points[volume.Points[2]].Vector3() + volumeFloorHeight;
                points[3] = area.Points[volume.Points[3]].Vector3() + volumeFloorHeight;
            }

            int barrelSegments = design.BarrelSegments + 1;
            Vector3[] bPoints = new Vector3[barrelSegments * 2];
            for (int i = 0; i < barrelSegments; i++)
            {
                float lerp = i / (float)(barrelSegments - 1);
                Vector3 height = Mathf.Sin(lerp * Mathf.PI) * design.Height * Vector3.up;
                float cosLerp = 1 - (Mathf.Cos((lerp) * Mathf.PI) + 1) / 2;
                bPoints[i] = Vector3.Lerp(points[0], points[1], cosLerp) + height;
                bPoints[i + barrelSegments] = Vector3.Lerp(points[2], points[3], cosLerp) + height;
            }

            int topIterations = barrelSegments - 1;
            int subMesh = design.GetTexture();
            bool flipped = design.IsFlipped();
            for (int t = 0; t < topIterations; t++)
                AddPlane(bPoints[t + 1], bPoints[t], bPoints[t + barrelSegments + 1], bPoints[t + barrelSegments], subMesh, flipped);//top

            Vector3 centerA = Vector3.Lerp(points[0], points[1], 0.5f);
            Vector3 centerB = Vector3.Lerp(points[2], points[3], 0.5f);
            for (int e = 0; e < topIterations; e++)
            {
                float lerpA = (e / (float)(topIterations)) * Mathf.PI;
                float lerpB = ((e + 1) / (float)(topIterations)) * Mathf.PI;
                Vector2[] uvs = new Vector2[3]{
				new Vector2(0.5f,0),
				new Vector2(1-(Mathf.Cos(lerpA)+1)/2,Mathf.Sin(lerpA)),
				new Vector2(1-(Mathf.Cos(lerpB)+1)/2,Mathf.Sin(lerpB))
			};

                Vector3[] verts = new Vector3[3] { centerA, bPoints[e], bPoints[e + 1] };
                int[] tri = new int[3] { 0, 2, 1 };
                AddData(verts, uvs, tri, design.GetTexture());

                verts = new Vector3[3] { centerB, bPoints[e + barrelSegments], bPoints[e + 1 + barrelSegments] };
                tri = new int[3] { 0, 1, 2 };
                AddData(verts, uvs, tri, design.GetTexture());
            }
        }

        private void LeanTo(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
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
            Vector3[] points = new Vector3[6];
            points[0] = area.Points[volume.Points[pointIndexes[0]]].Vector3() + volumeFloorHeight;
            points[1] = area.Points[volume.Points[pointIndexes[1]]].Vector3() + volumeFloorHeight;
            points[2] = area.Points[volume.Points[pointIndexes[2]]].Vector3() + volumeFloorHeight;
            points[3] = area.Points[volume.Points[pointIndexes[3]]].Vector3() + volumeFloorHeight;
            points[4] = area.Points[volume.Points[pointIndexes[2]]].Vector3() + volumeFloorHeight + ridgeVector;
            points[5] = area.Points[volume.Points[pointIndexes[3]]].Vector3() + volumeFloorHeight + ridgeVector;

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
            Texture texture = _textures[subMesh];

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
            if (!design.IsFlipped())
                uvs = new Vector2[3] { new Vector2(uvWdith, 0), new Vector2(0, 0), new Vector2(uvHeight, uvWdith / 2) };

            int[] triA = new int[3] { 1, 0, 2 };
            int[] triB = new int[3] { 0, 1, 2 };

            AddData(vertsA, uvs, triA, subMesh);
            AddData(vertsB, uvs, triB, subMesh);
        }

        private void Sawtooth(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
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
                Texture texture = _textures[subMesh];

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

        private void Steeple(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);
            Vector3 ridgeVector = Vector3.up * design.Height;

            int numberOfVolumePoints = volume.Points.Count;
            Vector3[] basePoints = new Vector3[numberOfVolumePoints];
            Vector3 centrePoint = Vector3.zero;
            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                basePoints[l] = area.Points[volume.Points[l]].Vector3() + volumeFloorHeight;
                centrePoint += area.Points[volume.Points[l]].Vector3();
            }
            centrePoint = (centrePoint / numberOfVolumePoints) + volumeFloorHeight + ridgeVector;
            for (int l = 0; l < numberOfVolumePoints; l++)
            {
                int pointIndexA = l;
                int pointIndexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                Vector3[] verts = new Vector3[3] { basePoints[pointIndexA], basePoints[pointIndexB], centrePoint };
                float uvWdith = Vector3.Distance(basePoints[pointIndexA], basePoints[pointIndexB]);
                float uvHeight = design.Height;
                int subMesh = design.GetTexture();
                Texture texture = _textures[subMesh];

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
                Vector2[] uvs = new Vector2[3] { new Vector2(-uvWdith / 2, 0), new Vector2(uvWdith / 2, 0), new Vector2(0, uvHeight) };
                int[] tri = new int[3] { 1, 0, 2 };
                AddData(verts, uvs, tri, subMesh);
            }
        }

        private void Hipped(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float baseHeight = _model.FloorHeight * numberOfFloors;
            float roofHeight = design.Height;
            int numberOfVolumePoints = volume.Points.Count;
            int subMesh = design.GetTexture();

            Vector2[] volumePoints = new Vector2[numberOfVolumePoints];
            for (int i = 0; i < numberOfVolumePoints; i++)
            {
                volumePoints[i] = area.Points[volume.Points[i]];
            }

            Vector2[][] meshData = StraightSkeleton.Calculate(volumePoints);
            Vector2[] triData = meshData[0];
            List<Vector2> interiorPoints = new List<Vector2>(meshData[1]);
            int numberOfVerts = triData.Length;
            Vector3[] verts = new Vector3[numberOfVerts];
            Vector2[] uvs = new Vector2[numberOfVerts];
            int[] tris = new int[numberOfVerts];
            for (int i = 0; i < triData.Length; i += 3)
            {
                Vector2 pa = triData[i];
                Vector2 pb = triData[i + 1];
                Vector2 pc = triData[i + 2];

                float ah = baseHeight + (interiorPoints.Contains(pa) ? roofHeight : 0);
                float bh = baseHeight + (interiorPoints.Contains(pb) ? roofHeight : 0);
                float ch = baseHeight + (interiorPoints.Contains(pc) ? roofHeight : 0);

                Vector3 v0 = new Vector3(pa.x, ah, pa.y);
                Vector3 v1 = new Vector3(pb.x, bh, pb.y);
                Vector3 v2 = new Vector3(pc.x, ch, pc.y);

                verts[i] = v0;
                verts[i + 1] = v1;
                verts[i + 2] = v2;

                Vector3 roofBaseDir = (interiorPoints.Contains(pc)) ? v1 - v0 : v2 - v1;
                Vector3 roofBaseNormal = Vector3.Cross(roofBaseDir, Vector3.up);
                Vector2[] uvsMansard = BuildingProjectUVs.Project(new Vector3[3] { v0, v1, v2 }, Vector2.zero, roofBaseNormal);

                uvs[i] = uvsMansard[0];
                uvs[i + 1] = uvsMansard[1];
                uvs[i + 2] = uvsMansard[2];

                tris[i] = i;
                tris[i + 1] = i + 2;
                tris[i + 2] = i + 1;
            }

            AddData(verts, uvs, tris, subMesh);
        }

        private void Gabled(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);
            Vector3 ridgeVector = Vector3.up * design.Height;

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
            centrePoint = (centrePoint / 4) + volumeFloorHeight + ridgeVector;

            Vector3 r0 = Vector3.Lerp(basePoints[0], basePoints[1], 0.5f) + ridgeVector;
            Vector3 r1 = Vector3.Lerp(basePoints[2], basePoints[3], 0.5f) + ridgeVector;

            int subMesh = design.GetTexture();
            bool flipped = design.IsFlipped();
            AddPlane(basePoints[0], r0, basePoints[3], r1, subMesh, flipped);//top
            AddPlane(basePoints[2], r1, basePoints[1], r0, subMesh, flipped);//top

            Vector3[] vertsA = new Vector3[3] { basePoints[0], basePoints[1], r0 };
            Vector3[] vertsB = new Vector3[3] { basePoints[2], basePoints[3], r1 };
            float uvWdithA = Vector3.Distance(basePoints[0], basePoints[1]);
            float uvWdithB = Vector3.Distance(basePoints[2], basePoints[3]);
            float uvHeight = design.Height;
            subMesh = design.GetTexture();
            Texture texture = _textures[subMesh];

            if (texture.Tiled)
            {
                uvWdithA *= (1.0f / texture.TextureUnitSize.x);
                uvWdithB *= (1.0f / texture.TextureUnitSize.x);
                uvHeight *= (1.0f / texture.TextureUnitSize.y);
                if (texture.Patterned)
                {
                    Vector2 uvunits = texture.TileUnitUV;
                    uvWdithA = Mathf.Ceil(uvWdithA / uvunits.x) * uvunits.x;
                    uvWdithB = Mathf.Ceil(uvWdithB / uvunits.x) * uvunits.x;
                    uvHeight = Mathf.Ceil(uvHeight / uvunits.y) * uvunits.y;
                }
            }
            else
            {
                uvWdithA = texture.TiledX;
                uvWdithB = texture.TiledX;
                uvHeight = texture.TiledY;
            }
            Vector2[] uvsA = new Vector2[3] { new Vector2(-uvWdithA / 2, 0), new Vector2(uvWdithA / 2, 0), new Vector2(0, uvHeight) };
            Vector2[] uvsB = new Vector2[3] { new Vector2(-uvWdithB / 2, 0), new Vector2(uvWdithB / 2, 0), new Vector2(0, uvHeight) };
            int[] tri = new int[3] { 1, 0, 2 };
            AddData(vertsA, uvsA, tri, subMesh);
            AddData(vertsB, uvsB, tri, subMesh);

        }

        private void Parapet(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int volumeIndex = area.Volumes.IndexOf(volume);
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

            for (int l = 0; l < numberOfVolumePoints; l++)
            {

                int indexA, indexB, indexA0, indexB0;
                Vector3 p0, p1, p00, p10;
                indexA = l;
                indexB = (l < numberOfVolumePoints - 1) ? l + 1 : 0;
                indexA0 = (l > 0) ? l - 1 : numberOfVolumePoints - 1;
                indexB0 = (l < numberOfVolumePoints - 2) ? l + 2 : l + 2 - numberOfVolumePoints;

                int adjacentFloorHeight = area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexA], volume.Points[indexB]);
                bool leftParapet = area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexA0], volume.Points[indexA]) < numberOfFloors;
                bool rightParapet = area.GetFacadeFloorHeight(volumeIndex, volume.Points[indexB], volume.Points[indexB0]) < numberOfFloors;

                if (adjacentFloorHeight >= numberOfFloors)
                    continue;//do not draw a roof edge

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
                Vector3 pr = facadeDirection * facadeWidth;
                Vector3 pu = Vector3.up * parapetHeight;

                Vector3 pbdl, pbdr, pfdl, pfdr;
                if (leftParapet)
                {
                    pbdl = -(facadeNormal + facadeNormalLeft).normalized * parapetFrontDepth;
                    pfdl = (facadeNormal + facadeNormalLeft).normalized * parapetBackDepth;
                }
                else
                {
                    pbdl = facadeDirectionLeft * parapetFrontDepth;
                    pfdl = -facadeDirectionLeft * parapetBackDepth;
                }
                if (rightParapet)
                {
                    pbdr = -(facadeNormal + facadeNormalRight).normalized * parapetFrontDepth;
                    pfdr = (facadeNormal + facadeNormalRight).normalized * parapetBackDepth;
                }
                else
                {
                    pbdr = -facadeDirectionRight * parapetFrontDepth;
                    pfdr = facadeDirectionRight * parapetBackDepth;
                }

                p0 += volumeFloorHeight;
                

                w2 = p0 + pbdl;//front left
                w3 = p0 + pr + pbdr;//front right
                w0 = p0 + pfdl;//back left
                w1 = p0 + pr + pfdr;//back right
                w6 = p0 + pbdl + pu;//front left top
                w7 = p0 + pr + pbdr + pu;//front right top
                w4 = p0 + pfdl + pu;//back left top
                w5 = p0 + pr + pfdr + pu;//back right top

                int subMesh = design.GetTexture();
                bool flipped = design.IsFlipped();
                AddPlane(w1, w0, w5, w4, subMesh, flipped);//front
                AddPlaneComplex(w5, w4, w7, w6, subMesh, facadeNormal);//top
                AddPlane(w2, w3, w6, w7, subMesh, flipped);//back

                if (parapetFrontDepth > 0)
                    AddPlaneComplex(w0, w1, w2, w3, subMesh, facadeNormal);//bottom

                if (!leftParapet)
                    AddPlane(w0, w2, w4, w6, subMesh, flipped);//left cap

                if (!rightParapet)
                    AddPlane(w3, w1, w7, w5, subMesh, flipped);//left cap
            }
        }

        private void Dormers(Volume volume, Roof design)
        {
            Plan area = _model.Plan;
            int numberOfVolumePoints = volume.Points.Count;
            int numberOfFloors = volume.NumberOfFloors;
            float floorHeight = _model.FloorHeight;
            Vector3 volumeFloorHeight = Vector3.up * (numberOfFloors * floorHeight);

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

                float windowBottom = (design.Height - design.DormerHeight) * design.DormerHeightRatio;

                float baseDepth = design.FloorDepth;
                float cornerLeftRad = Vector3.Angle(facadeDirection, -facadeDirectionLeft) * Mathf.Deg2Rad / 2;
                float cornerRightRad = Vector3.Angle(-facadeDirection, facadeDirectionRight) * Mathf.Deg2Rad / 2;
                float cornerDepthLeft = baseDepth / Mathf.Sin(cornerLeftRad);
                float cornerDepthRight = baseDepth / Mathf.Sin(cornerRightRad);
                float topDepth = design.Depth;
                float cornerTopDepthLeft = topDepth / Mathf.Sin(cornerLeftRad);
                float cornerTopDepthRight = topDepth / Mathf.Sin(cornerRightRad);

                float dormerDepth = design.Depth * (design.DormerHeight / design.Height);
                float windowBottomRat = Mathf.Lerp(0, 1 - design.DormerHeight / design.Height, design.DormerHeightRatio);

                p0 += volumeFloorHeight + leftDir * cornerDepthLeft;
                p1 += volumeFloorHeight + rightDir * cornerDepthRight;

                float leftStartTopRad = Vector3.Angle(facadeDirectionLeft, facadeDirection) * Mathf.Deg2Rad * 0.5f;
                float leftStartMargin = cornerTopDepthLeft * Mathf.Sin(leftStartTopRad);

                float rightStartTopRad = Vector3.Angle(facadeDirection, facadeDirectionRight) * Mathf.Deg2Rad * 0.5f;
                float rightStartMargin = cornerTopDepthRight * Mathf.Sin(rightStartTopRad);

                Vector3 dormerStartPosition = leftDir * (windowBottomRat * cornerTopDepthLeft) + facadeDirection * (leftStartMargin);
                Vector3 dormerEndPosition = rightDir * (windowBottomRat * cornerTopDepthRight) - facadeDirection * (rightStartMargin + design.DormerWidth);
                float dormerPositionWidth = Vector3.Distance((p0 + dormerStartPosition), (p1 + dormerEndPosition));
                int numberOfWindows = Mathf.FloorToInt((dormerPositionWidth) / (design.DormerWidth + design.MinimumDormerSpacing));
                float actualWindowSpacing = (dormerPositionWidth - (numberOfWindows * design.DormerWidth)) / (numberOfWindows + 1);
                numberOfWindows++;//add the final window

                Vector3 dormerWidthVector = facadeDirection * design.DormerWidth;
                Vector3 dormerHeightVectorA = Vector3.up * (design.DormerHeight - design.DormerRoofHeight);
                Vector3 dormerHeightVectorB = Vector3.up * design.DormerHeight;
                Vector3 dormerDepthVector = facadeNormal * dormerDepth;
                Vector3 dormerSpace = facadeDirection * (actualWindowSpacing + design.DormerWidth);
                Vector3 dormerSpacer = facadeDirection * (actualWindowSpacing);
                Vector3 dormerYPosition = Vector3.up * windowBottom;

                Vector3 w0, w1, w2, w3, w4, w5, w6, w7, w8, w9;
                for (int i = 0; i < numberOfWindows; i++)
                {
                    w0 = p0 + dormerSpace * (i) + dormerStartPosition + dormerYPosition + dormerSpacer * 0.5f;
                    w1 = w0 + dormerWidthVector;
                    w2 = w0 + dormerHeightVectorA;
                    w3 = w1 + dormerHeightVectorA;
                    w4 = w0 + dormerWidthVector / 2 + dormerHeightVectorB;

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

                    AddPlane(w1, w6, w3, w8, subMeshwall, flippedwall);//side
                    AddPlane(w5, w0, w7, w2, subMeshwall, flippedwall);//side
                    AddPlane(w3, w8, w4, w9, subMeshtiles, flippedtiles);//roof
                    AddPlane(w7, w2, w9, w4, subMeshtiles, flippedtiles);//roof

                    Vector3[] verts = new Vector3[5] { w0, w1, w2, w3, w4 };
                    float roofBottom = (design.DormerHeight - design.DormerRoofHeight) / design.DormerHeight;
                    Vector2[] uvs = new Vector2[5]{
					new Vector2(0,0),
					new Vector2(1,0),
					new Vector2(0,roofBottom),
					new Vector2(1,roofBottom),
					new Vector2(0.5f,1)
                    };
                    int[] tris = new int[9] { 1, 0, 2, 1, 2, 3, 2, 4, 3 };
                    _mesh.AddData(verts, uvs, tris, subMeshwindow);
                }
            }
        }

        private void AddData(Vector3[] verts, Vector2[] uvs, int[] tris, int subMesh)
        {
            _mesh.AddData(verts, uvs, tris, subMesh);
        }

        private void AddPlane(Vector3 w0, Vector3 w1, Vector3 w2, Vector3 w3, int subMesh, bool flipped)
        {
            int textureSubmesh = subMesh;
            Texture texture = _textures[textureSubmesh];
            Vector2 uvSize = Vector2.one;
            if (texture.Tiled)
            {
                float planeWidth = Vector3.Distance(w0, w1);
                float planeHeight = Vector3.Distance(w0, w2);
                uvSize = new Vector2(planeWidth * (1.0f / texture.TextureUnitSize.x), planeHeight * (1.0f / texture.TextureUnitSize.y));
                if (texture.Patterned)
                {
                    Vector2 uvunits = texture.TileUnitUV;
                    uvSize.x = Mathf.Ceil(uvSize.x / uvunits.x) * uvunits.x;
                    uvSize.y = Mathf.Ceil(uvSize.y / uvunits.y) * uvunits.y;
                }
            }
            else
            {
                uvSize.x = texture.TiledX;
                uvSize.y = texture.TiledY;
            }
            if (!flipped)
            {
                _mesh.AddPlane(w0, w1, w2, w3, Vector2.zero, uvSize, textureSubmesh);
            }
            else
            {
                uvSize = new Vector2(uvSize.y, uvSize.x);
                _mesh.AddPlane(w0, w1, w2, w3, Vector2.zero, uvSize, textureSubmesh);
            }
        }

        private void AddPlaneComplex(Vector3 w0, Vector3 w1, Vector3 w2, Vector3 w3, int subMesh, Vector3 facadeNormal)
        {
            Vector3[] verts = new Vector3[4] { w0, w1, w2, w3 };
            Vector2[] uvs = BuildingProjectUVs.Project(verts, Vector2.zero, facadeNormal);
            int[] tris = new int[6] { 1, 0, 2, 1, 2, 3 };

            _mesh.AddData(verts, uvs, tris, subMesh);
        }
    }
}
