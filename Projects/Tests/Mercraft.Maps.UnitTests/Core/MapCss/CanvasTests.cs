using System.Collections.Generic;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Core.MapCss
{
    [TestFixture]
    public class CanvasTests
    {
        [Test]
        public void CanReadTextureLine()
        {
            // ARRANGE
            var stylesheet =
                MapCssHelper.GetStylesheet(
                    "canvas { z-index: 0.1; texture: 0, MyTexture0, 10, 10; texture: 1, MyTexture1, 20, 20}\n");
            var canvas = MapCssHelper.GetCanvas();

            // ACT
            var rule = stylesheet.GetCanvasRule(canvas);
            var textureDeclarations = rule.EvaluateList<List<string>>("texture");

            // ASSERT
            Assert.AreEqual(2, textureDeclarations.Count);
            Assert.AreEqual(4, textureDeclarations[0].Count);
            Assert.AreEqual("MyTexture0", textureDeclarations[0][1]);
            Assert.AreEqual(4, textureDeclarations[1].Count);
            Assert.AreEqual("MyTexture1", textureDeclarations[1][1]);
        }
    }
}