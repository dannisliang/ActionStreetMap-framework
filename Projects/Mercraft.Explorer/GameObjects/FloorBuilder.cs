using System;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class FloorBuilder : IFloorBuilder
    {
        private readonly string LogTag = typeof (FloorBuilder).Name;

        [Dependency]
        private ITrace Trace { get; set; }
        
        public GameObject Build(Tile tile)
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.position = new Vector3(tile.TileMapCenter.x, 0, tile.TileMapCenter.y);
            floor.transform.localScale = new Vector3(tile.Size, 1, tile.Size);

            floor.name = "floor";

            Trace.Info(LogTag, String.Format("Created floor on position: {0}, size: {1}", 
                floor.transform.position, tile.Size));

            AttachTexture(floor);

            return floor;
        }

        private void AttachTexture(GameObject gameObject)
        {
            System.Random r = new System.Random(DateTime.Now.Millisecond);
            var color = new Color(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material.shader = Shader.Find("Bumped Diffuse");
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0, 0, color);
            texture2D.Apply();
            meshRenderer.material.mainTexture = (Texture)texture2D;
            meshRenderer.material.color = color;
           
            MeshHelper.TangentSolver(gameObject.GetComponent<MeshFilter>().mesh);
        }
    }
}
