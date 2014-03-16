using System.Collections.Generic;
using Mercraft.Core.Utilities;
using Mercraft.Explorer.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mercraft.Explorer.Builders.Areas.Generators
{
    /// <summary>
    /// Builds soviet union style building.
    /// </summary>
    public class SovietBuildingGenerator
    {
        #region Mesh Models

        private List<Mesh> balcony25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony25Glazed")
        };

        private List<Mesh> balcony30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony30Glazed")
        };

        private List<Mesh> wall25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Wall25")
        };
        private List<Mesh> wall30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Wall30")
        };
        private List<Mesh> window25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window25")
        };
        private List<Mesh> window30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window30")
        };

        private List<Mesh> socle25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Socle25")
        };
        private List<Mesh> socle30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Socle30")
        };

        private List<Mesh> entrance25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance25Roofed"),
        };
        private List<Mesh> entrance30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance30Roofed"),
        };
        private List<Mesh> entranceWall25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWall25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window25")
        };
        private List<Mesh> entranceWall30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWall30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window30")
        };
        private List<Mesh> entranceWallLast25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast25")
        };
        private List<Mesh> entranceWallLast30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast30")
        };
        private List<Mesh> attic25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Attic25")
        };
        private List<Mesh> attic30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Attic30")
        };
        private List<Mesh> roofFlat = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Roof")
        };
        private List<Mesh> roofGabled = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/RoofGabled")
        };
        private List<Mesh> roofHipped = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/RoofHipped")
        };

        #endregion

        // TODO develop randomize strategy
        private readonly System.Random random = new System.Random();
        
        private float[] panels = { 3, 2.5f };

        public void Generate(GameObject gameObject, Vector2[] verticies, BuildingSettings settings)
        {
            
            var meshFilter = gameObject.GetComponent<MeshFilter>();

            var combine = new List<Mesh>();
            var matrices = new List<Matrix4x4>();

            var height = settings.CeilingHeight * settings.Levels + settings.SocleHeight;

            for (int i = 0; i < verticies.Length - 1; i++)
            {
                var start = verticies[i];
                var end = verticies[i + 1];

                var length = Vector2.Distance(start, end);

                float resultWidth;
                var wallSizes1 = ExteriorWallSizes(length, out resultWidth);
                var panelPattern1 = FacadePattern(wallSizes1.Count, settings.Levels, settings, settings.Attic, false);

                var corner1 = new Vector3(start.x, 0, start.y);
                var direction = end - start;

                combine.Add(Facade(corner1, Vector3.Normalize(new Vector3(direction.x, 0, direction.y)), wallSizes1, panelPattern1, settings));
                matrices.Add(Matrix4x4.identity);

                // build missing parts
                var emptySpace = length - resultWidth;
                if (emptySpace > Mathf.Epsilon)
                {
                    combine.Add(CreatePolygon(end, direction, emptySpace, height));
                    matrices.Add(Matrix4x4.identity);
                }

            }

            meshFilter.mesh.Clear();
            meshFilter.mesh = PGMesh.CombineMeshes(combine, matrices);
            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.Optimize();
            gameObject.renderer.material.color = new Color(Random.value, Random.value, Random.value);
        }

        private List<float> ExteriorWallSizes(float wallLength, out float resultWidth)
        {
            resultWidth = 0f;
            var draft = ExteriorWallSizesDraft(wallLength);
            var wallSizes = new List<float>();
            for (var i = 0; i < draft.Length; i++)
            {
                for (var j = 0; j < draft[i]; j++)
                {
                    var panel = panels[i];
                    resultWidth += panel;
                    wallSizes.Add(panel);
                }
            }
            wallSizes.Shuffle();
            return wallSizes;
        }

        private int[] ExteriorWallSizesDraft(float remainder, int[] draft = null, int startIndex = 0)
        {
            if (draft == null)
            {
                draft = new int[panels.Length];
                for (int i = 0; i < draft.Length; i++)
                {
                    draft[i] = 0;
                }
            }
            if (remainder < panels[panels.Length - 1])
            {
                draft[draft.Length - 1] = 1;
                return draft;
            }
            for (var i = startIndex; i < panels.Length; i++)
            {
                draft[i] += (int)(remainder / panels[i]);
                remainder %= panels[i];
            }
            if (remainder > 0)
            {
                for (var i = 0; i < draft.Length; i++)
                {
                    if (draft[i] != 0)
                    {
                        if (i == draft.Length - 1)
                        {
                            return draft;
                        }
                        draft[i]--;
                        remainder += panels[i];
                        startIndex = i + 1;
                        break;
                    }
                }
                draft = ExteriorWallSizesDraft(remainder, draft, startIndex);
            }
            return draft;
        }

        private Mesh CreatePolygon(Vector2 start, Vector2 direction, float width, float height)
        {
            Vector2 end = start - (direction.normalized * width);
            var mesh = new Mesh();

            var vertices = new Vector3[]
            {
               new Vector3(start.x, 0, start.y),
               new Vector3(end.x, 0, end.y),
               new Vector3(start.x, height, start.y),
               new Vector3(end.x, height, end.y),
            };

            var uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(1, 1),
            };

            int[] triangles = new int[]
            {
                0, 1, 2,
                2, 1, 3,
            };


            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            return mesh;
        }

        Mesh Facade(Vector3 origin, Vector3 direction, List<float> wallSizes, List<List<PanelType>> panelPattern, BuildingSettings settings)
        {
            var floorMeshes = new List<Mesh>();
            var facadeMeshes = new List<Mesh>();
            var matrices = new List<Matrix4x4>();
            var panelOrigin = origin;

            for (var i = 0; i < panelPattern.Count; i++)
            {
                for (var j = 0; j < panelPattern[i].Count; j++)
                {
                    if (Mathf.Abs(wallSizes[j] - 2.5f) < Mathf.Epsilon)
                    {
                        switch (panelPattern[i][j])
                        {
                            case PanelType.Window:
                                floorMeshes.Add(RandomItem(window25));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Balcony:
                                floorMeshes.Add(RandomItem(balcony25));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Wall:
                                floorMeshes.Add(RandomItem(wall25));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Socle:
                                floorMeshes.Add(RandomItem(socle25));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Entrance:
                                floorMeshes.Add(entrance25[settings.EntranceMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.EntranceWall:
                                floorMeshes.Add(entranceWall25[settings.EntranceWallMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin + Vector3.up * (settings.CeilingHeight - settings.SocleHeight), Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.EntranceWallLast:
                                floorMeshes.Add(entranceWallLast25[settings.EntranceWallLastMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin + Vector3.up * (settings.CeilingHeight - settings.SocleHeight), Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Attic:
                                floorMeshes.Add(RandomItem(attic25));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                        }
                    }
                    else
                    {
                        switch (panelPattern[i][j])
                        {
                            case PanelType.Window:
                                floorMeshes.Add(RandomItem(window30));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Balcony:
                                floorMeshes.Add(RandomItem(balcony30));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Wall:
                                floorMeshes.Add(RandomItem(wall30));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Socle:
                                floorMeshes.Add(RandomItem(socle30));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Entrance:
                                floorMeshes.Add(entrance30[settings.EntranceMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.EntranceWall:
                                floorMeshes.Add(entranceWall30[settings.EntranceWallMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin + Vector3.up * (settings.CeilingHeight - settings.SocleHeight), Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.EntranceWallLast:
                                floorMeshes.Add(entranceWallLast30[settings.EntranceWallLastMeshIndex]);
                                matrices.Add(Matrix4x4.TRS(panelOrigin + Vector3.up * (settings.CeilingHeight - settings.SocleHeight), Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                            case PanelType.Attic:
                                floorMeshes.Add(RandomItem(attic30));
                                matrices.Add(Matrix4x4.TRS(panelOrigin, Quaternion.LookRotation(Vector3.Cross(direction, Vector3.up)), Vector3.one));
                                break;
                        }
                    }
                    panelOrigin += direction * wallSizes[j];
                }
                facadeMeshes.Add(PGMesh.CombineMeshes(floorMeshes, matrices));
                floorMeshes.Clear();
                matrices.Clear();
                panelOrigin = origin + Vector3.up * (i * settings.CeilingHeight + settings.SocleHeight);
            }

            return PGMesh.CombineMeshes(facadeMeshes);
        }

        List<List<PanelType>> FacadePattern(int panelCount, int floorCount, BuildingSettings settings, bool haveAttic = false, bool longFacade = false, int entrancesCount = 0)
        {
            var panelPattern = new List<List<PanelType>>();
            var entranceIndex = panelCount / (settings.Entrances + 1);
            var entranceCount = 1;

            for (var i = 0; i < floorCount + 1; i++)
            {
                panelPattern.Add(new List<PanelType>());
                for (var j = 0; j < panelCount; j++)
                {
                    if (i == 0)
                    {
                        if (entrancesCount > 0 && j == entranceIndex && entranceCount <= settings.Entrances)
                        {
                            panelPattern[0].Add(PanelType.Entrance);
                            entranceCount++;
                            entranceIndex = panelCount * entranceCount / (settings.Entrances + 1);
                        }
                        else
                        {
                            panelPattern[0].Add(PanelType.Socle);
                        }
                    }
                    else if (i == 1)
                    {
                        if (panelPattern[0][j] == PanelType.Entrance)
                        {
                            panelPattern[1].Add(PanelType.EntranceWall);
                        }
                        else if (longFacade)
                        {
                            panelPattern[1].Add(PanelType.Window);
                        }
                        else
                        {
                            panelPattern[1].Add(PanelType.Wall);
                        }
                    }
                    else
                    {
                        panelPattern[i].Add(panelPattern[i - 1][j]);
                    }
                    if (i == floorCount)
                    {
                        if (panelPattern[i - 1][j] == PanelType.Entrance || panelPattern[i - 1][j] == PanelType.EntranceWall)
                        {
                            panelPattern[i][j] = PanelType.EntranceWallLast;
                        }
                    }
                }
                if (i == 1 && !longFacade)
                {
                    for (int j = 0; j <= panelPattern[1].Count / 2; j++)
                    {
                        if (j != 0 && j != panelCount - 1 && Random.value > 0.5f)
                        {
                            panelPattern[1][j] = PanelType.Window;
                            panelPattern[1][panelPattern[1].Count - 1 - j] = PanelType.Window;
                        }
                    }
                }
                if (i == 2)
                {
                    for (int j = 0; j <= panelPattern[2].Count / 2; j++)
                    {
                        if (panelPattern[2][j] == PanelType.Window && panelPattern[2][panelPattern[2].Count - 1 - j] == PanelType.Window && Random.value > 0.5f)
                        {
                            panelPattern[2][j] = PanelType.Balcony;
                            panelPattern[2][panelPattern[2].Count - 1 - j] = PanelType.Balcony;
                        }
                    }
                }
            }
            if (haveAttic)
            {
                panelPattern.Add(new List<PanelType>());
                for (var j = 0; j < panelCount; j++)
                {
                    panelPattern[panelPattern.Count - 1].Add(PanelType.Attic);
                }
            }
            return panelPattern;
        }

        T RandomItem<T>(IList<T> itemList)
        {
            //return itemList[random.Next(itemList.Count)];
            return itemList[0];
        }
    
    }
}
