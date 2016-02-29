using NUnit.Framework;
using Torrent.Infrastructure.Media;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Media.Tests
{
    [TestFixture()]
    public class HSBColorTests
    {
        private HSBColor target;
        [SetUp]
        public void Setup()
        {
            target = new HSBColor();
        }

        [Test()]
        public void AdjustBrightnessTest()
        {
            Assert.DoesNotThrow(() => target.AdjustBrightness(-1));
            Assert.DoesNotThrow(() => target.AdjustBrightness(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AdjustBrightness(-1.0001));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.AdjustBrightness(1.0001));
        }

        [Test()]
        public void SetBrightnessTest()
        {
            Assert.DoesNotThrow(() => target.SetBrightness(0));
            Assert.DoesNotThrow(() => target.SetBrightness(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.SetBrightness(-0.0001));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.SetBrightness(1.0001));
        }
    }
}