namespace Torrent.Infrastructure.Media
{
    using System;
    using System.Windows.Media;
    using PostSharp.Patterns.Contracts;
    /// <summary>
    /// Structure to define HSB.
    /// </summary>
    public struct HSB
    {
        /// <summary>
        /// Gets an empty HSB structure;
        /// </summary>
        public static readonly HSB Empty = new HSB();

        private double hue;
        private double saturation;
        private double brightness;

        public static bool operator ==(HSB item1, HSB item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Brightness == item2.Brightness
                );
        }

        public static bool operator !=(HSB item1, HSB item2)
        {
            return (
                item1.Hue != item2.Hue
                || item1.Saturation != item2.Saturation
                || item1.Brightness != item2.Brightness
                );
        }

        /// <summary>
        /// Gets or sets the hue component.
        /// </summary>
        [Range(0, 360)]
        public double Hue
        {
            get
            {
                return hue;
            }
            set
            {
                hue = (value > 360) ? 360 : ((value < 0) ? 0 : value);
            }
        }

        /// <summary>
        /// Gets or sets saturation component.
        /// </summary>
        public double Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                saturation = (value > 1) ? 1 : ((value < 0) ? 0 : value);
            }
        }

        /// <summary>
        /// Gets or sets the brightness component.
        /// </summary>
        public double Brightness
        {
            get
            {
                return brightness;
            }
            set
            {
                brightness = (value > 1) ? 1 : ((value < 0) ? 0 : value);
            }
        }

        /// <summary>
        /// Creates an instance of a HSB structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="b">Brightness value.</param>
        public HSB(double h, double s, double b)
        {
            hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            brightness = (b > 1) ? 1 : ((b < 0) ? 0 : b);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HSB)obj);
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Brightness.GetHashCode();
        }
    }

    //Assert.DoesNotThrow(() => target.AdjustBrightness(-1));
    //        Assert.DoesNotThrow(() => target.AdjustBrightness(1));
    //        Assert.Throws<ArgumentOutOfRangeException>(() => target.AdjustBrightness(-1.0001));
    //        Assert.Throws<ArgumentOutOfRangeException>(() => target.AdjustBrightness(1.0001));
        
    //        Assert.DoesNotThrow(() => target.SetBrightness(0));
    //        Assert.DoesNotThrow(() => target.SetBrightness(1));
    //        Assert.Throws<ArgumentOutOfRangeException>(() => target.SetBrightness(-0.0001));
    //        Assert.Throws<ArgumentOutOfRangeException>(() => target.SetBrightness(1.0001));
    public class HSBColor
    {
        public double H { get; set; }
        public double S { get; set; }
        [Range(0d,1d)]
        public double B { get; set; }
        public byte A { get; set; }

        /// <summary>
        /// Adjust Brightness by adding the provided difference
        /// </summary>
        /// <param name="difference">Should be between -1 and 1</param>
        /// <returns></returns>
        public HSBColor AdjustBrightness([Range(-1d, 1d)] double difference)
            => SetBrightness(B + difference);

        /// <summary>
        /// Explicitly set Brightness
        /// </summary>
        /// <param name="brightness">Should be between 0 and 1</param>
        /// <returns></returns>
        public HSBColor SetBrightness([Range(0d, 1d)] double brightness)
        {
            B = brightness;
            return this;
        }
        public static HSBColor FromColor(Color rgbColor)
        {
            var result = new HSBColor
            {
                // preserve alpha
                A = rgbColor.A
            };


            // convert R, G, B to numbers from 0 to 1
            var r = rgbColor.R / 255d;
            var g = rgbColor.G / 255d;
            var b = rgbColor.B / 255d;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            // hue
            if (max == min)
                result.H = 0;
            else if (max == r)
                result.H = (60 * (g - b) / (max - min) + 360) % 360;
            else if (max == g)
                result.H = 60 * (b - r) / (max - min) + 120;
            else
                result.H = 60 * (r - g) / (max - min) + 240;

            // saturation
            if (max == 0)
                result.S = 0;
            else
                result.S = 1 - min / max;

            // brightness
            result.B = max;

            return result;
        }

        public Color ToColor()
        {
            if (B < 0 || B > 1)
                throw new InvalidOperationException("Brightness should be between 0 and 1");

            var result = new Color
            {
                A = this.A
            };


            var hi = (int)Math.Floor(this.H / 60) % 6;
            var f = this.H / 60 - Math.Floor(this.H / 60);

            var p = this.B * (1 - this.S);
            var q = this.B * (1 - f * this.S);
            var t = this.B * (1 - (1 - f) * this.S);

            switch (hi)
            {
                case 0:
                    result.R = (byte)(this.B * 255);
                    result.G = (byte)(t * 255);
                    result.B = (byte)(p * 255);
                    break;
                case 1:
                    result.R = (byte)(q * 255);
                    result.G = (byte)(this.B * 255);
                    result.B = (byte)(p * 255);
                    break;
                case 2:
                    result.R = (byte)(p * 255);
                    result.G = (byte)(this.B * 255);
                    result.B = (byte)(t * 255);
                    break;
                case 3:
                    result.R = (byte)(p * 255);
                    result.G = (byte)(q * 255);
                    result.B = (byte)(this.B * 255);
                    break;
                case 4:
                    result.R = (byte)(t * 255);
                    result.G = (byte)(p * 255);
                    result.B = (byte)(this.B * 255);
                    break;
                case 5:
                    result.R = (byte)(this.B * 255);
                    result.G = (byte)(p * 255);
                    result.B = (byte)(q * 255);
                    break;
            }

            return result;
        }
    }
    
    
}