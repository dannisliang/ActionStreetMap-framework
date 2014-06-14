using UnityEngine;

namespace Mercraft.Models.Roads.Builders
{
    /// <summary>
    ///     Simple road builder
    /// http://8bitmemories.blogspot.de/2011/10/procedural-track-generation-in-unity3d.html
    /// </summary>
    public class SimpleRoadBuilder
    {
        public static void Build(GameObject gameObject, RoadSettings settings)
        {
            // Rail
            float railheight = 0;
            float railwidth = 0;
            var points = settings.Points;
            var width = settings.Width;
            var height = settings.Height;

            // Create mesh filter
            var meshfilter = gameObject.AddComponent(typeof (MeshFilter)) as MeshFilter;

            // Create and set the mesh
            var mesh = new Mesh();
            meshfilter.mesh = mesh;

            // Blocks
            int n = points.Length - 1;

            // 18 + x8 vertices
            var vs = new Vector3[18 + (n - 1)*8];

            // iterator
            int run = 0;

            float w = 0;
            float h = 0;
            for (int s = 0; s < 8; s++)
            {
                // For the given sequence, set w and h offset from the center
                switch (s)
                {
                    case 0:
                        w = -width/2f;
                        h = height/2f;
                        break;

                    case 1:
                        w = width/2f;
                        h = height/2f;
                        break;

                    case 2:
                        w = width/2f;
                        h = height/2f + railheight;
                        break;

                    case 3:
                        w = width/2f + railwidth;
                        h = height/2f + railheight;
                        break;

                    case 4:
                        w = width/2f + railwidth;
                        h = -height/2f;
                        break;

                    case 5:
                        w = -width/2f - railwidth;
                        h = -height/2f;
                        break;

                    case 6:
                        w = -width/2f - railwidth;
                        h = height/2f + railheight;
                        break;

                    case 7:
                        w = -width/2f;
                        h = height/2f + railheight;
                        break;

                    default:
                        break;
                }

                // Default initialize - cribbing compiler
                Vector3 fwd = Vector3.forward, left = Vector3.left;

                for (int i = 0; i <= n; i++)
                {
                    // Except for the last point
                    if (i != n)
                    {
                        // Direction of track
                        fwd = points[i + 1] - points[i];

                        // Now assume no banking
                        fwd.y = 0;
                        fwd.Normalize();

                        // Get left
                        left = Vector3.Cross(Vector3.up, fwd);
                    }

                    vs[run++] = points[i] + left*w + Vector3.up*h;
                }
            }

            mesh.vertices = vs;

            //TODO: Do your UV mapping here

            // Triangle n x 16 x 3
            int[] triangles = new int[n*16*3];

            // reset iterator
            run = 0;
            for (int s = 0; s < 8; s++)
            {
                for (int i = 0; i < n; i++)
                {
                    // 1st Tri
                    triangles[run + 0] = s*(n + 1) + i;
                    triangles[run + 1] = s*(n + 1) + i + 1;
                    triangles[run + 2] = ((s + 1)%8)*(n + 1) + i;

                    // 2nd Tri
                    triangles[run + 3] = s*(n + 1) + i + 1;
                    triangles[run + 4] = ((s + 1)%8)*(n + 1) + i + 1;
                    triangles[run + 5] = ((s + 1)%8)*(n + 1) + i;

                    run += 6;
                }
            }

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            //? mesh.Optimize();

            // Add collider
            gameObject.AddComponent(typeof (MeshCollider));
            gameObject.AddComponent<MeshRenderer>();

            gameObject.renderer.material = settings.Material;
            gameObject.renderer.material.color = settings.Color;
        }
    }
}