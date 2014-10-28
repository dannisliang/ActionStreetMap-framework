using System.Collections.Generic;
using Mercraft.Core.Scene.World.Buildings;
using Mercraft.Core.Unity;
using Mercraft.Core.Utilities;
using Mercraft.Explorer.Themes;
using Mercraft.Models.Buildings;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer.Themes
{
    [TestFixture]
    public class BuildingTextureBalancerTests
    {
        [Test]
        public void CanChooseBestFacadeByColor()
        {
            // ARRANGE
            IBuildingStyleProvider provider = CreateProvider();

            // ACT & ASSERT
            Assert.AreEqual(ColorUtility.FromName("red"), provider.Get(new Building()
            {
                Type = "residential",
                FacadeColor = ColorUtility.FromName("red"),
            }).Facade.Color, "Unable to use color name");

            Assert.AreEqual(ColorUtility.FromUnknown("#4C2F20"), provider.Get(new Building()
            {
                Type = "residential",
                FacadeColor = ColorUtility.FromUnknown("#4C2F27"),
            }).Facade.Color, "Unable to use color from hex");
        }

        [Test]
        public void CanChooseBestRoofByColor()
        {
            // ARRANGE
            IBuildingStyleProvider provider = CreateProvider();

            // ACT & ASSERT
            Assert.AreEqual(ColorUtility.FromName("red"), provider.Get(new Building()
            {
                Type = "residential",
                Id = 1,
                RoofColor = ColorUtility.FromName("red"),
            }).Roof.Color, "Cannot choose roof");
        }

        private IBuildingStyleProvider CreateProvider()
        {
            var facadeStyles = new List<BuildingStyle.FacadeStyle>()
            {
                new BuildingStyle.FacadeStyle()
                {
                    Height = 0,
                    Color = ColorUtility.FromName("red"),
                },
                new BuildingStyle.FacadeStyle()
                {
                    Height = 12,
                    Color = ColorUtility.FromUnknown("#4C2F20"),
                },
            };

            var roofStyles = new List<BuildingStyle.RoofStyle>()
            {
                new BuildingStyle.RoofStyle()
                {
                    Type = "dom",
                    Material = "brick",
                    Color = ColorUtility.FromName("red")
                },
                new BuildingStyle.RoofStyle()
                {
                    Type = "dom",
                    Material = "glass",
                    Color = ColorUtility.FromName("white")
                }
            };

            return new BuildingStyleProvider(
                new Dictionary<string, List<BuildingStyle.FacadeStyle>>()
                {
                    {"residential", facadeStyles}
                },
                new Dictionary<string, List<BuildingStyle.RoofStyle>>()
                {
                    {"residential", roofStyles}
                });
        }
    }
}
