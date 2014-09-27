using System.Collections.Generic;
using Mercraft.Core.Utilities;
using Mercraft.Core.World.Buildings;
using Mercraft.Explorer.Themes;
using Mercraft.Models.Buildings;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Explorer.Themes
{
    [TestFixture]
    public class BuildingTextureBalancerTests
    {
        [Test]
        public void CanChooseBestFacade()
        {
            // ARRANGE
            IBuildingStyleProvider provider = CreateProvider();

            // ACT & ASSERT
            Assert.AreEqual(5, provider.Get(new Building()
            {
                Id = 1,
                Type = "residential",
                Levels = -1,
                FacadeMaterial = "glass",
            }).Facade.Height, "Cannot choose material");

            Assert.AreEqual(0, provider.Get(new Building()
            {
                Type = "residential",
                Levels = -1,
                FacadeMaterial = "brick",
                FacadeColor = ColorUtility.FromName("red"),
            }).Facade.Height, "Cannot choose colored material");
            
            Assert.AreEqual(12, provider.Get(new Building()
            {
                Type = "residential",
                Levels = -1,
                FacadeColor = ColorUtility.FromName("white"),
            }).Facade.Height, "Cannot choose color");

            Assert.AreEqual(12, provider.Get(new Building()
            {
                Type = "residential",
                Levels = 12,
            }).Facade.Height, "Cannot choose floors");
        }

        [Test]
        public void CanChooseBestRoof()
        {
            // ARRANGE
            IBuildingStyleProvider provider = CreateProvider();

            // ACT & ASSERT
            Assert.AreEqual("gable", provider.Get(new Building()
            {
                Type = "residential",
                Id = 1,
                RoofType = "gable",
                RoofMaterial  = "glass",
            }).Roof.Type, "Cannot choose roof");
        }

        private IBuildingStyleProvider CreateProvider()
        {
            var facadeStyles = new List<BuildingStyle.FacadeStyle>()
            {
                new BuildingStyle.FacadeStyle()
                {
                    Height = 9,
                    Material = "brick",
                },
                new BuildingStyle.FacadeStyle()
                {
                    Height = 5,
                    Material = "glass",
                },
                new BuildingStyle.FacadeStyle()
                {
                    Height = 0,
                    Material = "brick",
                    Color = ColorUtility.FromName("red"),
                },
                new BuildingStyle.FacadeStyle()
                {
                    Height = 12,
                    Material = "glass",
                    Color = ColorUtility.FromName("white"),
                },
            };

            var roofStyles = new List<BuildingStyle.RoofStyle>()
            {
                new BuildingStyle.RoofStyle()
                {
                    Type = "dom",
                    Material = "brick",
                },
                new BuildingStyle.RoofStyle()
                {
                    Type = "gable",
                    Material = "glass",
                },
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
