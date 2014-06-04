using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Infrastructure.Config;
using NUnit.Framework;

namespace Mercraft.Maps.UnitTests.Infrastructure
{
    [TestFixture]
    public class ConfigTests
    {
        private IConfigSection _stubSection;
        
        [TestFixtureSetUp]
        public void Initialize()
        {
            var config = new ConfigSettings("test.config", TestHelper.GetPathResolver());
            _stubSection = config.GetSection("stubs");
        }

        #region Elementary

        [Test]
        public void CanReadStringValue()
        {
            var value = _stubSection.GetString("string");
            Assert.AreEqual("string_value", value);
        }

        [Test]
        public void CanReadStringAttr()
        {
            var value = _stubSection.GetString("string/@attr");
            Assert.AreEqual("string_attr", value);
        }

        [Test]
        public void CanReadIntValue()
        {
            var value = _stubSection.GetInt("int");
            Assert.AreEqual(55, value);
        }

        [Test]
        public void CanReadIntAttr()
        {
            var value = _stubSection.GetInt("int/@attr");
            Assert.AreEqual(5, value);
        }

        [Test]
        public void CanReadFloatValue()
        {
            var value = _stubSection.GetFloat("float");
            Assert.IsTrue(Compare(5.12f, value));
        }

        [Test]
        public void CanReadFloatAttr()
        {
            var value = _stubSection.GetFloat("float/@attr");
            Assert.IsTrue(Compare(5.1f, value));
        }

        #endregion

        #region Helpers

        public static bool Compare(float a, float b)
        {
            return Math.Abs(a - b) < float.Epsilon;
        }

        #endregion
    }
}
