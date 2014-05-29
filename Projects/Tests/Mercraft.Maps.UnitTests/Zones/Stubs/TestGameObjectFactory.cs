using System;
using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestGameObjectFactory: IGameObjectFactory
    {
        public IGameObject CreateNew()
        {
            return new TestGameObject();
        }

        public IGameObject CreatePrimitive(PrimitiveType type)
        {
            return new TestGameObject();
        }
    }
}
