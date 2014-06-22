using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Models.Utils;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    /// <summary>
    /// Generates and fills alphamap of TerrainData using TerrainSettings provided
    /// </summary>
    public class AlphaMapGenerator
    {
        private readonly TerrainSettings _settings;
        private readonly Vector2 _terrainPosition;

        private float _widthRatio;
        private float _heightRatio;

        private TerrainData _terrainData;
        private AlphaMapElement[] _polygons;

        public AlphaMapGenerator(TerrainSettings settings)
        {
            _settings = settings;
            _terrainPosition = _settings.CornerPosition;
        }

        public void FillAlphaMap(TerrainData terrainData)
        {
            _terrainData = terrainData;
            var layers = _settings.SplatPrototypes.Length;
            _widthRatio = _settings.AlphaMapSize/terrainData.size.x;
            _heightRatio = _settings.AlphaMapSize / terrainData.size.z;

            CreatePolygons();

            float[,,] map = new float[_settings.AlphaMapSize, _settings.AlphaMapSize, layers];

            for (int x = 0; x < _settings.AlphaMapSize; x++)
            {
                for (int y = 0; y < _settings.AlphaMapSize; y++)
                {
                    var splatIndecies = GetPolygonMainSplatIndex(new Vector2(x, y)).ToArray();
                    // TODO process different layers with blending
                    // set default tecture
                    if (splatIndecies.Length == 0)
                    {
                        map[x, y, 0] = 1;
                    }
                    else
                    {
                        foreach (var splatIndex in splatIndecies)
                        {
                            map[x, y, splatIndex] = 1;
                        }
                    }
                }
            }

            terrainData.alphamapResolution = _settings.AlphaMapSize;
            terrainData.SetAlphamaps(0, 0, map);
        }

        private void CreatePolygons()
        {
            var areas = ConvertPolygon(_settings.Polygons);
            var curves = ConvertLine(_settings.Curves, 5f);

            var tempPolygons = new List<AlphaMapElement>(areas.Length + curves.Length);
            tempPolygons.AddRange(areas);
            tempPolygons.AddRange(curves);

            _polygons = tempPolygons.OrderBy(p => p.SplatIndex).ToArray();
        }

        private IEnumerable<int> GetPolygonMainSplatIndex(Vector2 point)
        {
            for (int i = 0; i < _polygons.Length; i++)
            {
                if (PolygonUtils.IsPointInPolygon(_polygons[i].Points, point))
                {
                    yield return _polygons[i].SplatIndex;
                } 
            }
        }

        private AlphaMapElement[] ConvertPolygon(TerrainElement[] elements)
        {
            return elements.Select(e => new AlphaMapElement()
            {
                ZIndex = e.ZIndex,
                SplatIndex = e.SplatIndex, // TODO use name in source and map it to index here
                Points = e.Points.Select(p => ConvertWorldToTerrain(p)).ToArray()
            }).ToArray();
        }

        private AlphaMapElement[] ConvertLine(TerrainElement[] elements, float width)
        {
            // make polygon from line
            var result = new List<AlphaMapElement>(elements.Length);
            foreach (var terrainElement in elements)
            {
                //var middlePoints = terrainElement.Points.Select(p => ConvertWorldToTerrain(p)).ToArray();
                var leftPoints = new List<Vector2>(terrainElement.Points.Length);
                var rightPoints = new List<Vector2>(terrainElement.Points.Length);
                for (int i = 0; i < terrainElement.Points.Length - 1; i++)
                {
                    var first = new Vector2(terrainElement.Points[i].X, terrainElement.Points[i].Y);
                    var second = new Vector2(terrainElement.Points[i+1].X, terrainElement.Points[i+1].Y);
                    var newVec = first - second;
                    var newVector = Vector3.Cross(newVec, Vector3.forward);
                    newVector.Normalize();

                    var newVector2 = new Vector2(newVector.x, newVector.z);
                    var newPoint = width*newVector2 + first;
                    var newPoint2 = -width*newVector2 + first;

                    leftPoints.Add(newPoint);
                    rightPoints.Add(newPoint2);

                    Debug.Log(String.Format("{0} {1} {2}", first, newPoint, newPoint2));

                    if (i == terrainElement.Points.Length - 2)
                    {
                        var newPoint3 = new Vector2(newPoint.x, second.y);
                        var newPoint4 = new Vector2(newPoint2.x, second.y);
                        leftPoints.Add(newPoint3);
                        rightPoints.Add(newPoint4);
                    }
                }

                var resultPoints = new List<Vector2>(terrainElement.Points.Length*2);
                resultPoints.AddRange(leftPoints);
                rightPoints.Reverse();
                resultPoints.AddRange(rightPoints);
                result.Add(new AlphaMapElement()
                {
                    ZIndex = terrainElement.ZIndex,
                    SplatIndex = terrainElement.SplatIndex,
                    // TODO use name in source and map it to index here
                    Points = resultPoints.Select(p => ConvertWorldToTerrain(p)).ToArray()
                });
            }
            return result.ToArray();
        }

        private Vector2 ConvertWorldToTerrain(Vector2 worldPos)
        {
            return ConvertWorldToTerrain(worldPos.x, worldPos.y);
        }

        private Vector2 ConvertWorldToTerrain(MapPoint worldPos)
        {
            return ConvertWorldToTerrain(worldPos.X, worldPos.Y);
        }

        private Vector2 ConvertWorldToTerrain(float x, float y)
        {
            return new Vector2
            {
                // NOTE worldPos is inverted here
                x = (y - _terrainPosition.x) * _widthRatio,
                y = (x - _terrainPosition.y) * _heightRatio
            };
        }
    }
}
