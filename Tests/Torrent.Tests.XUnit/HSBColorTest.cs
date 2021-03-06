// <copyright file="HSBColorTest.cs">Copyright 2015</copyright>
using System;
using System.Windows.Media;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Torrent.Infrastructure.Media;

namespace Torrent.Infrastructure.Media.Tests
{
    /// <summary>This class contains parameterized unit tests for HSBColor</summary>
    [PexClass(typeof(HSBColor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    public partial class HSBColorTest
    {
        /// <summary>Test stub for AdjustBrightness(Double)</summary>
        [PexMethod]
        public HSBColor AdjustBrightnessTest([PexAssumeUnderTest]HSBColor target, double difference)
        {
            HSBColor result = target.AdjustBrightness(difference);
            return result;
            // TODO: add assertions to method HSBColorTest.AdjustBrightnessTest(HSBColor, Double)
        }

        /// <summary>Test stub for FromColor(Color)</summary>
        [PexMethod]
        public HSBColor FromColorTest(Color rgbColor)
        {
            HSBColor result = HSBColor.FromColor(rgbColor);
            return result;
            // TODO: add assertions to method HSBColorTest.FromColorTest(Color)
        }

        /// <summary>Test stub for SetBrightness(Double)</summary>
        [PexMethod]
        public HSBColor SetBrightnessTest([PexAssumeUnderTest]HSBColor target, double brightness)
        {
            HSBColor result = target.SetBrightness(brightness);
            return result;
            // TODO: add assertions to method HSBColorTest.SetBrightnessTest(HSBColor, Double)
        }

        /// <summary>Test stub for ToColor()</summary>
        [PexMethod]
        public Color ToColorTest([PexAssumeUnderTest]HSBColor target)
        {
            Color result = target.ToColor();
            return result;
            // TODO: add assertions to method HSBColorTest.ToColorTest(HSBColor)
        }
    }
}
