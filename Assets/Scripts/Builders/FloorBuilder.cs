using UnityEngine;

namespace Mercraft.Scene.Builders
{
    public class FloorBuilder
    {
        const float Scale = 10;
        public void Build()
        {
            for (int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 4; x++)
                {
                    float offset = 0;
                        //(z % 2 == 0 ? .5f * Scale : 0f);
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * Scale + offset, 30, z * Scale);
                    cube.transform.localScale = new Vector3(Scale, 1, Scale);
                }
            }
        }
    }
}
