using System.Collections.Generic;
using Mercraft.Models.Buildings.Entities;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Builders
{
    public class BuilderFullDetail
    {
        private static Data data;
        private static Texture[] textures;
        private static DynamicMeshGenericMultiMaterialMesh mesh;

        public static void Build(DynamicMeshGenericMultiMaterialMesh _mesh, Data _data)
        {
            data = _data;
            mesh = _mesh;
            textures = data.Textures.ToArray();
            FacadeDesign facadeDesign = data.Facades[0];
            Plan plan = data.Plan;
            int numberOfVolumes = plan.numberOfVolumes;
            for (int v = 0; v < numberOfVolumes; v++)
            {
                Volume volume = plan.volumes[v];
                int numberOfVolumePoints = volume.points.Count;
                for (int f = 0; f < numberOfVolumePoints; f++)
                {
                    if (!volume.renderFacade[f])
                        continue;

                    int indexAM = Mathf.Abs((f - 1) % numberOfVolumePoints);
                    int indexA = f;
                    int indexB = (f + 1) % numberOfVolumePoints;
                    int indexBP = (f + 2) % numberOfVolumePoints;
                    Vector3 p0m = plan.points[volume.points[indexAM]].Vector3();
                    Vector3 p0 = plan.points[volume.points[indexA]].Vector3();
                    Vector3 p1 = plan.points[volume.points[indexB]].Vector3();
                    Vector3 p1p = plan.points[volume.points[indexBP]].Vector3();

                    float facadeWidth = Vector3.Distance(p0, p1);
                    Vector3 facadeDirection = (p1 - p0).normalized;
                    Vector3 facadeCross = Vector3.Cross(facadeDirection, Vector3.up);
                    Vector3 lastFacadeDirection = (p0 - p0m).normalized;
                    Vector3 nextFacadeDirection = (p1p - p1).normalized;

                    //only bother with facade directions when facade may intersect inverted geometry
                    float facadeDirDotL = Vector3.Dot(-facadeDirection, lastFacadeDirection);
                    float facadeCrossDotL = Vector3.Dot(-facadeCross, lastFacadeDirection);
                    if (facadeDirDotL <= 0 || facadeCrossDotL <= 0) lastFacadeDirection = -facadeCross;

                    float facadeDirDotN = Vector3.Dot(-facadeDirection, nextFacadeDirection);
                    float facadeCrossDotN = Vector3.Dot(-facadeCross, nextFacadeDirection);
                    if (facadeDirDotN <= 0 || facadeCrossDotN <= 0) nextFacadeDirection = facadeCross;


                    int floorBase = plan.GetFacadeFloorHeight(v, volume.points[indexA], volume.points[indexB]);
                    int numberOfFloors = volume.numberOfFloors - floorBase;
                    if (numberOfFloors < 1)
                    {
                        //no facade - adjacent facade is taller and covers this one
                        continue;
                    }
                    float floorHeight = data.FloorHeight;
                    Vector3 floorHeightStart = Vector3.up * (floorBase * floorHeight);
                    p0 += floorHeightStart;
                    VolumeStylesUnit[] styleUnits = volume.styles.GetContentsByFacade(volume.points[indexA]);
                    int floorPatternSize = 0;
                    List<int> facadePatternReference = new List<int>();//this contains a list of all the facade style indices to refence when looking for the appropriate style per floor
                    int patternCount = 0;
                    foreach (VolumeStylesUnit styleUnit in styleUnits)//need to knw how big all the styles are together so we can loop through them
                    {
                        floorPatternSize += styleUnit.floors;
                        for (int i = 0; i < styleUnit.floors; i++)
                            facadePatternReference.Add(patternCount);
                        patternCount++;
                    }
                    facadePatternReference.Reverse();

                    int rows = numberOfFloors;

                    float foundationHeight = data.FoundationHeight;
                    if (foundationHeight > 0)
                    {
                        //draw foundations
                        Bay foundationStyle = (facadeDesign.type == FacadeDesign.types.patterned) ? data.Bays[facadeDesign.bayPattern[0]] : facadeDesign.simpleBay;
                        int subMesh = foundationStyle.GetTexture(Bay.TextureNames.WallTexture);
                        bool flipped = foundationStyle.IsFlipped(Bay.TextureNames.WallTexture);
                        Vector3 foundationVector = Vector3.up * -foundationHeight;
                        Vector3 w0 = p0 + foundationVector;
                        Vector3 w1 = p1 + foundationVector;
                        Vector3 w2 = p0;
                        Vector3 w3 = p1;
                        Vector2 foundationUVEnd = new Vector2(facadeWidth, foundationHeight);
                        AddPlane(w0, w1, w2, w3, subMesh, flipped, Vector2.zero, foundationUVEnd);
                    }

                    Vector2 facadeUV = Vector2.zero;

                    for (int r = 0; r < rows; r++)
                    {
                        bool firstRow = r == 0;
                        bool lastRow = r == (rows - 1);
                        //Get the facade style id
                        //need to loop through the facade designs floor by floor until we get to the right one
                        float currentHeight = floorHeight * r;
                        Vector3 facadeFloorBaseVector = p0 + Vector3.up * currentHeight;
                        int modFloor = (r % floorPatternSize);
                        int modFloorPlus = ((r + 1) % floorPatternSize);
                        int modFloorMinus = (r > 0) ? ((r - 1) % floorPatternSize) : 0;
                        FacadeDesign lastFacadeDesign = null;
                        FacadeDesign nextFacadeDesign = null;

                        facadeDesign = data.Facades[styleUnits[facadePatternReference[modFloor]].styleID];
                        nextFacadeDesign = data.Facades[styleUnits[facadePatternReference[modFloorPlus]].styleID];
                        lastFacadeDesign = data.Facades[styleUnits[facadePatternReference[modFloorMinus]].styleID];

                        bool isBlankWall = !facadeDesign.hasWindows;
                        if (facadeDesign.type == FacadeDesign.types.patterned)
                        {
                            if (data.Bays.Count == 0 || facadeDesign.bayPattern.Count == 0)
                            {
                                data.Illegal = true;
                                return;
                            }

                            Bay firstBay = data.Bays[facadeDesign.bayPattern[0]];
                            if (firstBay.openingWidth > facadeWidth) isBlankWall = true;
                            if (facadeDesign.bayPattern.Count == 0) isBlankWall = true;
                        }
                        else
                        {
                            if (facadeDesign.simpleBay.openingWidth + facadeDesign.simpleBay.minimumBayWidth > facadeWidth)
                                isBlankWall = true;
                        }

                        if (!isBlankWall)
                        {
                            float patternSize = 0;//the space the pattern fills, there will be a gap that will be distributed to all bay styles
                            int numberOfBays = 0;
                            //float actualWindowSpacing;
                            Bay[] bayDesignPattern;
                            int numberOfBayDesigns;
                            if (facadeDesign.type == FacadeDesign.types.patterned)
                            {
                                numberOfBayDesigns = facadeDesign.bayPattern.Count;
                                bayDesignPattern = new Bay[numberOfBayDesigns];
                                for (int i = 0; i < numberOfBayDesigns; i++)
                                {
                                    bayDesignPattern[i] = data.Bays[facadeDesign.bayPattern[i]];
                                }
                            }
                            else
                            {
                                bayDesignPattern = new[] { facadeDesign.simpleBay };
                                numberOfBayDesigns = 1;
                            }
                            //start with first window width - we'll be adding to this until we have filled the facade width
                            int it = 100;
                            while (true)
                            {
                                int patternModIndex = numberOfBays % numberOfBayDesigns;
                                float patternAddition = bayDesignPattern[patternModIndex].openingWidth + bayDesignPattern[patternModIndex].minimumBayWidth;
                                if (patternSize + patternAddition < facadeWidth)
                                {
                                    patternSize += patternAddition;
                                    numberOfBays++;
                                }
                                else
                                    break;
                                it--;
                                if (it < 0)
                                    break;
                            }

                            Vector3 windowBase = facadeFloorBaseVector;
                            facadeUV.x = 0;
                            facadeUV.y += floorHeight;
                            float perBayAdditionalSpacing = (facadeWidth - patternSize) / numberOfBays;
                            for (int c = 0; c < numberOfBays; c++)
                            {
                                Bay bayStyle;
                                Bay lastBay;
                                Bay nextBay;
                                bool firstColumn = c == 0;
                                bool lastColumn = c == numberOfBays - 1;
                                if (facadeDesign.type == FacadeDesign.types.patterned)
                                {
                                    int numberOfBayStyles = facadeDesign.bayPattern.Count;
                                    bayStyle = bayDesignPattern[c % numberOfBayStyles];
                                    int lastBayIndex = (c > 0) ? (c - 1) % numberOfBayStyles : 0;
                                    lastBay = bayDesignPattern[lastBayIndex];
                                    nextBay = bayDesignPattern[(c + 1) % numberOfBayStyles];
                                }
                                else
                                {
                                    bayStyle = facadeDesign.simpleBay;
                                    lastBay = facadeDesign.simpleBay;
                                    nextBay = facadeDesign.simpleBay;
                                }
                                float actualWindowSpacing = bayStyle.minimumBayWidth + perBayAdditionalSpacing;
                                float leftWidth = actualWindowSpacing * bayStyle.openingWidthRatio;
                                float rightWidth = actualWindowSpacing - leftWidth;
                                float openingWidth = bayStyle.openingWidth;


                                //                            float openingHeight = bayStyle.openingHeight;
                                Texture columnTexture = textures[bayStyle.GetTexture(Bay.TextureNames.ColumnTexture)];
                                Vector2 columnuvunits = columnTexture.tileUnitUV;
                                float openingHeight = bayStyle.openingHeight;
                                if (columnTexture.patterned) openingHeight = Mathf.Ceil(bayStyle.openingHeight / columnuvunits.y) * columnuvunits.y;
                                if (bayStyle.openingHeight == floorHeight) bayStyle.openingHeight = floorHeight;

                                float rowBottomHeight = ((floorHeight - openingHeight) * bayStyle.openingHeightRatio);
                                if (columnTexture.patterned) rowBottomHeight = Mathf.Ceil(rowBottomHeight / columnuvunits.y) * columnuvunits.y;

                                float rowTopHeight = (floorHeight - rowBottomHeight - openingHeight);

                                bool previousBayIdentical = bayStyle == lastBay;
                                bool nextBayIdentical = bayStyle == nextBay;
                                if (previousBayIdentical && !firstColumn)
                                    leftWidth = actualWindowSpacing;//if next design is identical - add the two parts together the reduce polycount

                                Vector3 w0, w1, w2, w3;
                                int subMesh;
                                bool flipped;
                                float leftDepth, bottomDepth, rightDepth;

                                if (!bayStyle.isOpening)
                                {
                                    subMesh = bayStyle.GetTexture(Bay.TextureNames.WallTexture);
                                    flipped = bayStyle.IsFlipped(Bay.TextureNames.WallTexture);
                                    Vector3 bayWidth = facadeDirection * (openingWidth + actualWindowSpacing);
                                    Vector3 bayHeight = Vector3.up * floorHeight;
                                    Vector3 bayDepth = facadeCross;
                                    w0 = windowBase;
                                    w1 = windowBase + bayWidth;
                                    w2 = windowBase + bayHeight;
                                    w3 = windowBase + bayWidth + bayHeight;
                                    Vector2 bayOpeningUVEnd = facadeUV + new Vector2(openingWidth + actualWindowSpacing, floorHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, facadeUV, bayOpeningUVEnd);

                                    Vector2 UVEnd = new Vector2(1, floorHeight);
                                    if (!previousBayIdentical && !firstColumn)//left
                                    {
                                        Vector3 wA = w0 + bayDepth;
                                        Vector3 wB = w2 + bayDepth;
                                        AddPlane(w2, wB, w0, wA, subMesh, flipped, Vector2.zero, UVEnd);
                                    }

                                    if (!nextBayIdentical && !lastColumn)//right
                                    {
                                        Vector3 wA = w1 + bayDepth;
                                        Vector3 wB = w3 + bayDepth;
                                        AddPlane(w1, wA, w3, wB, subMesh, flipped, Vector2.zero, UVEnd);
                                    }

                                    if (lastFacadeDesign != facadeDesign && !firstRow)//bottom
                                    {
                                        Vector3 wA = w0 + ((!firstColumn) ? facadeCross : -lastFacadeDirection);
                                        Vector3 wB = w1 + ((!lastColumn) ? facadeCross : nextFacadeDirection);
                                        AddPlane(w0, wA, w1, wB, subMesh, flipped, Vector2.zero, UVEnd);
                                    }
                                    if (nextFacadeDesign != facadeDesign && !lastRow)//top
                                    {
                                        Vector3 wA = w2 + ((!firstColumn) ? facadeCross : -lastFacadeDirection);
                                        Vector3 wB = w3 + ((!lastColumn) ? facadeCross : nextFacadeDirection);
                                        AddPlane(w3, wB, w2, wA, subMesh, flipped, Vector2.zero, UVEnd);
                                    }

                                    windowBase = w1;//move base vertor to next bay
                                    facadeUV.x += openingWidth + actualWindowSpacing;
                                    continue;//bay filled - move onto next bay
                                }

                                var verts = new Vector3[16];
                                verts[0] = windowBase;
                                verts[1] = verts[0] + leftWidth * facadeDirection;
                                verts[2] = verts[1] + openingWidth * facadeDirection;
                                verts[3] = verts[2] + rightWidth * facadeDirection;
                                windowBase = (nextBayIdentical) ? verts[2] : verts[3];//move to next window - if next design is identical - well add the two parts together the reduce polycount
                                facadeUV.x += (nextBayIdentical) ? openingWidth : openingWidth + rightWidth;

                                Vector3 rowBottomVector = Vector3.up * rowBottomHeight;
                                verts[4] = verts[0] + rowBottomVector;
                                verts[5] = verts[1] + rowBottomVector;
                                verts[6] = verts[2] + rowBottomVector;
                                verts[7] = verts[3] + rowBottomVector;

                                Vector3 openingVector = Vector3.up * openingHeight;
                                verts[8] = verts[4] + openingVector;
                                verts[9] = verts[5] + openingVector;
                                verts[10] = verts[6] + openingVector;
                                verts[11] = verts[7] + openingVector;

                                Vector3 rowTopVector = Vector3.up * rowTopHeight;
                                verts[12] = verts[8] + rowTopVector;
                                verts[13] = verts[9] + rowTopVector;
                                verts[14] = verts[10] + rowTopVector;
                                verts[15] = verts[11] + rowTopVector;

                                Vector3 openingDepthVector = facadeCross * bayStyle.openingDepth;
                                Vector3 crossDepthVector = facadeCross * bayStyle.crossDepth;
                                Vector3 rowDepthVector = facadeCross * bayStyle.rowDepth;
                                Vector3 columnDepthVector = facadeCross * bayStyle.columnDepth;


                                //Window
                                subMesh = bayStyle.GetTexture(Bay.TextureNames.OpeningBackTexture);
                                flipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningBackTexture);
                                w0 = verts[5] + openingDepthVector;
                                w1 = verts[6] + openingDepthVector;
                                w2 = verts[9] + openingDepthVector;
                                w3 = verts[10] + openingDepthVector;
                                Vector2 windowUVStart = facadeUV + new Vector2(leftWidth, rowBottomHeight);
                                Vector2 windowUVEnd = windowUVStart + new Vector2(openingWidth, openingHeight);
                                AddPlane(w0, w1, w2, w3, subMesh, flipped, windowUVStart, windowUVEnd);

                                leftDepth = bayStyle.columnDepth - bayStyle.openingDepth;//Window Left
                                if (firstColumn) leftDepth = bayStyle.openingDepth;
                                if (leftDepth != 0)
                                {
                                    float leftWindowDepth = ((!firstColumn) ? leftDepth : -bayStyle.openingDepth);
                                    Vector3 leftDepthVector = facadeCross * leftWindowDepth;
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningSideTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningSideTexture);
                                    Vector2 uvStart = facadeUV + new Vector2(0, rowBottomHeight);
                                    Vector2 uvEnd = uvStart + new Vector2(leftWindowDepth, openingHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                }

                                bottomDepth = bayStyle.rowDepth - bayStyle.openingDepth;//Window Bottom
                                if (facadeDesign != lastFacadeDesign && rowBottomHeight == 0) bottomDepth = 1 - bayStyle.openingDepth;
                                if (rowBottomHeight == 0 && firstRow && foundationHeight > 0) bottomDepth = 0 - bayStyle.openingDepth;//foundation bottom
                                if (bottomDepth != 0 && ((floorHeight != openingHeight) || facadeDesign != lastFacadeDesign))
                                {
                                    if (facadeDesign != lastFacadeDesign && rowBottomHeight == 0) bottomDepth = 1 - bayStyle.openingDepth;
                                   
                                    Vector3 bottomDepthVector = facadeCross * bottomDepth;
                                    Vector3 wl0 = w0 + bottomDepthVector;
                                    Vector3 wl1 = w1 + bottomDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningSillTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningSillTexture);
                                    Vector2 uvEnd = new Vector2(openingWidth, bottomDepth);
                                    AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                if (rowTopHeight == 0 && lastRow)//Window Top
                                {
                                    Vector3 topDepthVector = ((!lastColumn) ? facadeCross : nextFacadeDirection) * (-bayStyle.openingDepth);
                                    Vector3 wl2 = w2 + topDepthVector;
                                    Vector3 wl3 = w3 + topDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                    Vector2 uvEnd = new Vector2(openingWidth, bottomDepth);
                                    AddPlane(wl3, wl2, w3, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                //Column
                                subMesh = bayStyle.GetTexture(Bay.TextureNames.ColumnTexture);
                                flipped = bayStyle.IsFlipped(Bay.TextureNames.ColumnTexture);
                                if (leftWidth > 0)//Column Left
                                {
                                    Vector3 leftColumeDepthVector = (!firstColumn) ? columnDepthVector : Vector3.zero;
                                    w0 = verts[4] + leftColumeDepthVector;
                                    w1 = verts[5] + leftColumeDepthVector;
                                    w2 = verts[8] + leftColumeDepthVector;
                                    w3 = verts[9] + leftColumeDepthVector;
                                    Vector2 leftColumnUVStart = facadeUV + new Vector2(0, rowBottomHeight);
                                    Vector2 leftColumnUVEnd = leftColumnUVStart + new Vector2(leftWidth, openingHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, leftColumnUVStart, leftColumnUVEnd);

                                    float leftPlaneDepth = (previousBayIdentical) ? bayStyle.openingDepth : 1;//Column Left Left
                                    leftDepth = leftPlaneDepth - bayStyle.columnDepth;
                                    if (leftDepth != 0 && !firstColumn)
                                    {
                                        Vector3 leftDepthVector = facadeCross * leftDepth;
                                        Vector3 wl0 = w0 + leftDepthVector;
                                        Vector3 wl1 = w2 + leftDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningSideTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningSideTexture);
                                        Vector2 uvStart = facadeUV + new Vector2(0, rowBottomHeight);
                                        Vector2 uvEnd = uvStart + new Vector2(leftDepth, openingHeight);
                                        AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                    }

                                    bottomDepth = bayStyle.crossDepth - bayStyle.columnDepth;//Column Left Bottom
                                    if (bottomDepth != 0 && rowTopHeight > 0 && rowBottomHeight > 0 && !firstColumn)
                                    {
                                        Vector3 bottomDepthVector = facadeCross * bottomDepth;
                                        Vector3 wl0 = w0 + bottomDepthVector;
                                        Vector3 wl1 = w1 + bottomDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningCeilingTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningCeilingTexture);
                                        Vector2 uvEnd = new Vector2(leftWidth, bottomDepth);
                                        AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }

                                }

                                Vector3 rightColumeDepthVector = (!lastColumn) ? columnDepthVector : Vector3.zero;//Column Right
                                w0 = verts[6] + rightColumeDepthVector;
                                w1 = verts[7] + rightColumeDepthVector;
                                w2 = verts[10] + rightColumeDepthVector;
                                w3 = verts[11] + rightColumeDepthVector;
                                if ((!nextBayIdentical || lastColumn) && rightWidth > 0)
                                {
                                    Vector2 rightColumnUVStart = facadeUV + new Vector2(leftWidth + openingWidth, rowBottomHeight);
                                    Vector2 rightColumnUVEnd = rightColumnUVStart + new Vector2(rightWidth, openingHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, rightColumnUVStart, rightColumnUVEnd);
                                }

                                leftDepth = bayStyle.openingDepth - bayStyle.columnDepth;//Column Right Left
                                if (lastColumn) leftDepth = bayStyle.openingDepth;
                                if (leftDepth != 0)
                                {
                                    Vector3 leftDepthVector = facadeCross * leftDepth;
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningSideTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningSideTexture);
                                    Vector2 uvStart = facadeUV + new Vector2(0, rowBottomHeight);
                                    Vector2 uvEnd = uvStart + new Vector2(leftDepth, openingHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                }

                                if (!nextBayIdentical && !lastColumn && rightWidth > 0)
                                {
                                    bottomDepth = bayStyle.crossDepth - bayStyle.columnDepth;//Column Right Bottom
                                    if (bottomDepth != 0 && rowTopHeight > 0 && rowBottomHeight > 0)
                                    {
                                        Vector3 bottomDepthVector = facadeCross * bottomDepth;
                                        Vector3 wl0 = w0 + bottomDepthVector;
                                        Vector3 wl1 = w1 + bottomDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.OpeningCeilingTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.OpeningCeilingTexture);
                                        Vector2 uvEnd = new Vector2(rightWidth, bottomDepth);
                                        AddPlane(wl1, w1, wl0, w0, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }

                                    rightDepth = 1 - bayStyle.columnDepth;//Column Right Right
                                    if (rightDepth > 0)
                                    {
                                        Vector3 rightDepthVector = facadeCross * rightDepth;
                                        Vector3 wl1 = w1 + rightDepthVector;
                                        Vector3 wl3 = w3 + rightDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.ColumnTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.ColumnTexture);
                                        Vector2 uvStart = facadeUV + new Vector2(0, rowBottomHeight);
                                        Vector2 uvEnd = uvStart + new Vector2(rightDepth, openingHeight);
                                        AddPlane(wl3, w3, wl1, w1, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                    }
                                }

                                //Row Bottom
                                subMesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                flipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                w0 = verts[1] + rowDepthVector;
                                w1 = verts[2] + rowDepthVector;
                                w2 = verts[5] + rowDepthVector;
                                w3 = verts[6] + rowDepthVector;
                                if (rowBottomHeight > 0)
                                {
                                    Vector2 bottomRowUVStart = facadeUV + new Vector2(leftWidth, 0);
                                    Vector2 bottomRowUVEnd = bottomRowUVStart + new Vector2(openingWidth, rowBottomHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, bottomRowUVStart, bottomRowUVEnd);
                                }

                                leftDepth = ((!firstColumn) ? bayStyle.crossDepth : 1) - bayStyle.rowDepth;//Row Bottom Left
                                if (leftDepth != 0)
                                {
                                    Vector3 leftDepthVector = facadeCross * ((!firstColumn) ? leftDepth : 0 - bayStyle.rowDepth);
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                    Vector2 uvStart = facadeUV + new Vector2(0, 0);
                                    Vector2 uvEnd = uvStart + new Vector2(leftDepth, rowBottomHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                }

                                bottomDepth = 1 - bayStyle.rowDepth;//Row Bottom Bottom
                                if (firstRow && foundationHeight > 0) bottomDepth = 0 - bayStyle.rowDepth;//foundation bottom
                                if (bottomDepth != 0 && (facadeDesign != lastFacadeDesign || firstRow) && rowBottomHeight > 0)
                                {
                                    Vector3 bottomDepthVector = facadeCross;
                                    bottomDepthVector *= bottomDepth;
                                    Vector3 wl0 = w0 + bottomDepthVector;
                                    Vector3 wl1 = w1 + bottomDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                    Vector2 uvEnd = new Vector2(bottomDepth, openingWidth);
                                    AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                //Row Top
                                w0 = verts[9] + rowDepthVector;
                                w1 = verts[10] + rowDepthVector;
                                w2 = verts[13] + rowDepthVector;
                                w3 = verts[14] + rowDepthVector;

                                if (rowTopHeight > 0)
                                {
                                    Vector2 topRowUVStart = facadeUV + new Vector2(leftWidth, rowBottomHeight + openingHeight);
                                    Vector2 topRowUVEnd = topRowUVStart + new Vector2(openingWidth, rowTopHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, topRowUVStart, topRowUVEnd);
                                }

                                //left depth doesn;t change from the bottom row - no need to recalculate
                                if (leftDepth != 0)//Row Top Left
                                {
                                    float leftRowDepth = ((!firstColumn) ? leftDepth : 0 - bayStyle.rowDepth);
                                    Vector3 leftDepthVector = facadeCross * leftRowDepth;
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                    Vector2 uvStart = facadeUV + new Vector2(0, rowTopHeight);
                                    Vector2 uvEnd = new Vector2(leftRowDepth, rowTopHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, uvStart, uvEnd);
                                }

                                bottomDepth = bayStyle.openingDepth - bayStyle.rowDepth;//Row Top Bottom
                                if (bottomDepth != 0 && ((floorHeight != openingHeight) || (lastRow && rowTopHeight > 0)))
                                {
                                    //Vector3 bottomDepthVector = ((!lastColumn) ? facadeCross : nextFacadeDirection) * bottomDepth;
                                    Vector3 bottomDepthVector = facadeCross * bottomDepth;
                                    Vector3 wl0 = w0 + bottomDepthVector;
                                    Vector3 wl1 = w1 + bottomDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                    Vector2 uvEnd = new Vector2(openingWidth, bottomDepth);
                                    AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                if ((lastRow || facadeDesign != nextFacadeDesign) && rowTopHeight > 0)
                                {
                                    float topPlaneRowDepth = 1 - bayStyle.rowDepth;//Row Top Top
                                    if (lastRow) topPlaneRowDepth = -bayStyle.rowDepth;//Row Top Top
                                    if (topPlaneRowDepth != 0)
                                    {
                                        Vector3 topDepthVector = ((!lastColumn) ? facadeCross : nextFacadeDirection) * topPlaneRowDepth;
                                        Vector3 wl2 = w2 + topDepthVector;
                                        Vector3 wl3 = w3 + topDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.RowTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.RowTexture);
                                        Vector2 uvEnd = new Vector2(openingWidth, topPlaneRowDepth);
                                        AddPlane(wl3, wl2, w3, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }
                                }


                                //Cross Left Bottom
                                subMesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                flipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                Vector3 leftCrossDepthVector = (!firstColumn) ? crossDepthVector : Vector3.zero;
                                w0 = verts[0] + leftCrossDepthVector;
                                w1 = verts[1] + leftCrossDepthVector;
                                w2 = verts[4] + leftCrossDepthVector;
                                w3 = verts[5] + leftCrossDepthVector;
                                Vector2 crossLBUVStart = facadeUV + new Vector2(0, 0);
                                Vector2 crossLBUVEnd = crossLBUVStart + new Vector2(leftWidth, rowBottomHeight);
                                AddPlane(w0, w1, w2, w3, subMesh, flipped, crossLBUVStart, crossLBUVEnd);

                                float leftPlaneCrossDepth = (previousBayIdentical) ? bayStyle.rowDepth : 1;//Cross Left Bottom Left
                                leftDepth = leftPlaneCrossDepth - bayStyle.crossDepth;
                                if (leftDepth != 0 && !firstColumn)
                                {
                                    Vector3 leftDepthVector = facadeCross * leftDepth;
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                    Vector2 uvEnd = new Vector2(leftDepth, rowBottomHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                bottomDepth = 1 - bayStyle.crossDepth;//Cross Left Bottom Bottom
                                if (bottomDepth != 0 && facadeDesign != lastFacadeDesign)
                                {
                                    Vector3 bottomDepthVector = ((!firstColumn) ? facadeCross : -lastFacadeDirection) * bottomDepth;
                                    Vector3 wl0 = w0 + bottomDepthVector;
                                    Vector3 wl1 = w1 + bottomDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                    Vector2 uvEnd = new Vector2(leftWidth, bottomDepth);
                                    AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                //Cross Left Top
                                w0 = verts[8] + leftCrossDepthVector;
                                w1 = verts[9] + leftCrossDepthVector;
                                w2 = verts[12] + leftCrossDepthVector;
                                w3 = verts[13] + leftCrossDepthVector;
                                Vector2 crossLTUVStart = facadeUV + new Vector2(0, rowBottomHeight + openingHeight);
                                Vector2 crossLTUVEnd = crossLTUVStart + new Vector2(leftWidth, rowTopHeight);
                                AddPlane(w0, w1, w2, w3, subMesh, flipped, crossLTUVStart, crossLTUVEnd);

                                if (leftDepth != 0 && !firstColumn)//Cross Left Top Left
                                {
                                    Vector3 leftDepthVector = facadeCross * leftDepth;
                                    Vector3 wl0 = w0 + leftDepthVector;
                                    Vector3 wl1 = w2 + leftDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                    Vector2 uvEnd = new Vector2(leftDepth, rowTopHeight);
                                    AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                bottomDepth = bayStyle.columnDepth - bayStyle.crossDepth;//Cross Left Top Bottom
                                if (bottomDepth != 0 && rowBottomHeight > 0 && !firstColumn)
                                {
                                    Vector3 bottomDepthVector = facadeCross * bottomDepth;
                                    Vector3 wl0 = w0 + bottomDepthVector;
                                    Vector3 wl1 = w1 + bottomDepthVector;
                                    int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                    bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                    Vector2 uvEnd = new Vector2(leftWidth, bottomDepth);
                                    AddPlane(wl0, wl1, w0, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                }

                                if ((lastRow || facadeDesign != nextFacadeDesign) && rowTopHeight > 0 && !firstColumn)
                                {
                                    float topPlaneCrossDepth = 1 - bayStyle.crossDepth;//Cross Left Top Top
                                    if (lastRow) topPlaneCrossDepth = -bayStyle.crossDepth;//Row Top Top
                                    if (topPlaneCrossDepth != 0)
                                    {
                                        Vector3 topCross = ((!firstColumn) ? facadeCross : lastFacadeDirection);
                                        Vector3 topDepthVector = topCross * topPlaneCrossDepth;
                                        Vector3 wl2 = w2 + topDepthVector;
                                        Vector3 wl3 = w3 + topDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                        Vector2 uvEnd = new Vector2(leftWidth, topPlaneCrossDepth);
                                        AddPlane(wl3, wl2, w3, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }
                                }

                                //Cross Right
                                if ((!nextBayIdentical || lastColumn) && rightWidth > 0)
                                {
                                    if (lastColumn) crossDepthVector = Vector3.zero;
                                    //Cross Right Bottom
                                    w0 = verts[2] + crossDepthVector;
                                    w1 = verts[3] + crossDepthVector;
                                    w2 = verts[6] + crossDepthVector;
                                    w3 = verts[7] + crossDepthVector;
                                    Vector2 crossRBUVStart = facadeUV + new Vector2(leftWidth + openingWidth, 0);
                                    Vector2 crossRBUVEnd = crossRBUVStart + new Vector2(rightWidth, rowBottomHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, crossRBUVStart, crossRBUVEnd);

                                    leftDepth = (!lastColumn) ? bayStyle.rowDepth - bayStyle.crossDepth : bayStyle.rowDepth;//Cross Right Bottom Left
                                    if (leftDepth != 0)
                                    {
                                        Vector3 leftDepthVector = facadeCross * leftDepth;
                                        Vector3 wl0 = w0 + leftDepthVector;
                                        Vector3 wl1 = w2 + leftDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                        Vector2 uvEnd = new Vector2(leftDepth, rowBottomHeight);
                                        AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }

                                    //if(!lastColumn)
                                    //{
                                    bottomDepth = 1 - bayStyle.crossDepth;//Cross Right Bottom Bottom
                                    if (bottomDepth != 0 && facadeDesign != lastFacadeDesign)
                                    {
                                        Vector3 bottomCross = ((!lastColumn) ? facadeCross : nextFacadeDirection);
                                        Vector3 wl0 = w0 + bottomCross * bottomDepth;
                                        Vector3 wl1 = w1 + bottomCross * bottomDepth;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                        Vector2 uvEnd = new Vector2(rightWidth, bottomDepth);
                                        AddPlane(wl1, w1, wl0, w0, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }
                                    //}

                                    if (!nextBayIdentical && !lastColumn)//Cross Right Bottom Right
                                    {
                                        rightDepth = 1 - bayStyle.crossDepth;
                                        if (rightDepth != 0)
                                        {
                                            Vector3 rightDepthVector = facadeCross * rightDepth;
                                            Vector3 wl1 = w1 + rightDepthVector;
                                            Vector3 wl3 = w3 + rightDepthVector;
                                            int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                            bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                            Vector2 uvEnd = new Vector2(rightDepth, rowBottomHeight);
                                            AddPlane(wl3, w3, wl1, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                        }
                                    }

                                    //Cross Right Top
                                    w0 = verts[10] + crossDepthVector;
                                    w1 = verts[11] + crossDepthVector;
                                    w2 = verts[14] + crossDepthVector;
                                    w3 = verts[15] + crossDepthVector;
                                    Vector2 crossRTUVStart = facadeUV + new Vector2(leftWidth + openingWidth, rowBottomHeight + openingHeight);
                                    Vector2 crossRTUVEnd = crossRTUVStart + new Vector2(rightWidth, rowTopHeight);
                                    AddPlane(w0, w1, w2, w3, subMesh, flipped, crossRTUVStart, crossRTUVEnd);

                                    leftDepth = (!lastColumn) ? bayStyle.rowDepth - bayStyle.crossDepth : bayStyle.rowDepth;//Cross Right Top left
                                    if (leftDepth != 0)
                                    {
                                        Vector3 leftDepthVector = facadeCross * leftDepth;
                                        Vector3 wl0 = w0 + leftDepthVector;
                                        Vector3 wl1 = w2 + leftDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                        Vector2 uvEnd = new Vector2(leftDepth, rowTopHeight);
                                        AddPlane(wl0, w0, wl1, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }

                                    bottomDepth = (!lastColumn) ? bayStyle.columnDepth - bayStyle.crossDepth : 0;//Cross Right Top bottom
                                    if (bottomDepth != 0)
                                    {
                                        Vector3 bottomCross = ((!lastColumn) ? facadeCross : -nextFacadeDirection);
                                        Vector3 bottomDepthVector = bottomCross * bottomDepth;
                                        Vector3 wl0 = w0 + bottomDepthVector;
                                        Vector3 wl1 = w1 + bottomDepthVector;
                                        int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                        bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                        Vector2 uvEnd = new Vector2(rightWidth, bottomDepth);
                                        AddPlane(wl1, w1, wl0, w0, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                    }

                                    if ((lastRow || facadeDesign != nextFacadeDesign) && !lastColumn)//Cross Right Top top
                                    {
                                        float topPlaneCrossDepth = 1 - bayStyle.crossDepth;
                                        if (topPlaneCrossDepth != 0)
                                        {
                                            Vector3 topCross = ((!lastColumn) ? facadeCross : -nextFacadeDirection);
                                            Vector3 topDepthVector = topCross * topPlaneCrossDepth;
                                            Vector3 wl2 = w2 + topDepthVector;
                                            Vector3 wl3 = w3 + topDepthVector;
                                            int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                            bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                            Vector2 uvEnd = new Vector2(rightWidth, topPlaneCrossDepth);
                                            AddPlane(wl3, wl2, w3, w2, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                        }
                                    }

                                    if (!nextBayIdentical && !lastColumn)//Cross Right Top right
                                    {
                                        rightDepth = 1 - bayStyle.crossDepth;
                                        if (rightDepth != 0)
                                        {
                                            Vector3 rightDepthVector = facadeCross * rightDepth;
                                            Vector3 wl1 = w1 + rightDepthVector;
                                            Vector3 wl3 = w3 + rightDepthVector;
                                            int windowBoxSubmesh = bayStyle.GetTexture(Bay.TextureNames.CrossTexture);
                                            bool windowBoxFlipped = bayStyle.IsFlipped(Bay.TextureNames.CrossTexture);
                                            Vector2 uvEnd = new Vector2(rightDepth, rowTopHeight);
                                            AddPlane(wl3, w3, wl1, w1, windowBoxSubmesh, windowBoxFlipped, Vector2.zero, uvEnd);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // windowless wall
                            Vector3 wallVector = (facadeDirection * facadeWidth);
                            Vector3 wallHeightVector = Vector3.up * floorHeight;
                            Vector3 w0 = facadeFloorBaseVector;
                            Vector3 w1 = facadeFloorBaseVector + wallVector;
                            Vector3 w2 = facadeFloorBaseVector + wallHeightVector;
                            Vector3 w3 = facadeFloorBaseVector + wallVector + wallHeightVector;
                            Texture texture = textures[facadeDesign.simpleBay.GetTexture(Bay.TextureNames.WallTexture)];
                            var uvSize = new Vector2(facadeWidth * (1.0f / texture.textureUnitSize.x), floorHeight * (1.0f / texture.textureUnitSize.y));
                            Vector2 uvunits = texture.tileUnitUV;
                            uvSize.x = Mathf.Ceil(uvSize.x / uvunits.x) * uvunits.x;
                            uvSize.y = Mathf.Ceil(uvSize.y / uvunits.y) * uvunits.y;
                            int wallSubmesh = facadeDesign.simpleBay.GetTexture(Bay.TextureNames.WallTexture);
                            bool flipped = facadeDesign.simpleBay.IsFlipped(Bay.TextureNames.WallTexture);
                            Vector2 wallUVStart = facadeUV;
                            Vector2 wallUVEnd = facadeUV + new Vector2(facadeWidth, floorHeight);
                            AddPlane(w0, w1, w2, w3, wallSubmesh, flipped, wallUVStart, wallUVEnd);

                            if (nextFacadeDesign.hasWindows && !lastRow)
                            {
                                Vector3 wl2 = w2 - lastFacadeDirection;
                                Vector3 wl3 = w3 + nextFacadeDirection;
                                Vector2 uvEnd = new Vector2(facadeWidth, 1);
                                AddPlane(w3, wl3, w2, wl2, wallSubmesh, flipped, Vector2.zero, uvEnd);
                            }
                        }
                    }
                }
                //Bottom of the mesh - it's mostly to ensure the model can render certain shadows correctly
                if (data.DrawUnderside)
                {
                    Vector3 foundationDrop = Vector3.down * data.FoundationHeight;
                    var newEndVerts = new Vector3[numberOfVolumePoints];
                    var newEndUVs = new Vector2[numberOfVolumePoints];
                    for (int i = 0; i < numberOfVolumePoints; i++)
                    {
                        newEndVerts[i] = plan.points[volume.points[i]].Vector3() + foundationDrop;
                        newEndUVs[i] = Vector2.zero;
                    }
                    var tris = new List<int>(data.Plan.GetTrianglesBySectorBase(v));
                    tris.Reverse();
                    int bottomSubMesh = facadeDesign.GetTexture(FacadeDesign.textureNames.columnTexture);
                    mesh.AddData(newEndVerts, newEndUVs, tris.ToArray(), bottomSubMesh);
                }
            }
            data = null;
            mesh = null;
            textures = null;
        }

        private static void AddPlane(Vector3 w0, Vector3 w1, Vector3 w2, Vector3 w3, int subMesh, bool flipped, Vector2 facadeUVStart, Vector2 facadeUVEnd)
        {
            int textureSubmesh = subMesh;
            Texture texture = textures[textureSubmesh];
            Vector2 uvStart = facadeUVStart;
            Vector2 uvEnd = facadeUVEnd;

            if (texture.tiled)
            {
                uvStart = new Vector2(facadeUVStart.x * (1.0f / texture.textureUnitSize.x), facadeUVStart.y * (1.0f / texture.textureUnitSize.y));
                uvEnd = new Vector2(facadeUVEnd.x * (1.0f / texture.textureUnitSize.x), facadeUVEnd.y * (1.0f / texture.textureUnitSize.y));
                if (texture.patterned)
                {
                    Vector2 uvunits = texture.tileUnitUV;
                    uvStart.x = Mathf.Max(Mathf.Floor(uvStart.x / uvunits.x), 0) * uvunits.x;
                    uvStart.y = Mathf.Max(Mathf.Floor(uvStart.y / uvunits.y), 0) * uvunits.y;
                    uvEnd.x = Mathf.Max(Mathf.Ceil(uvEnd.x / uvunits.x), 1) * uvunits.x;
                    uvEnd.y = Mathf.Max(Mathf.Ceil(uvEnd.y / uvunits.y), 1) * uvunits.y;
                }
            }
            else
            {
                uvStart = Vector2.zero;
                uvEnd.x = texture.tiledX;
                uvEnd.y = texture.tiledY;
            }

            if (!flipped)
                mesh.AddPlane(w0, w1, w2, w3, uvStart, uvEnd, textureSubmesh);
            else
            {
                uvStart = new Vector2(uvStart.y, uvStart.x);
                uvEnd = new Vector2(uvEnd.y, uvEnd.x);
                mesh.AddPlane(w2, w0, w3, w1, uvStart, uvEnd, textureSubmesh);
            }
        }
    }
}
