using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Interop;
using System.Xml.Serialization;
using static Torrent.Helpers.Utils.DebugUtils;
namespace wUAL.Properties.Settings.AppSettings
{
    using Serialization;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Puchalapalli.Extensions.Collections;
    using Puchalapalli.Infrastructure.Interfaces;
    using Torrent.Extensions;
    using Torrent.Infrastructure;
    [Serializable]
    [XmlSerializerAssembly("wUAL.Serializers")]
    [DataContract(Namespace = Namespaces.Default)]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT : IDebuggerDisplay
    {
        [DataMember]
        public int Left { get; set; }
        [DataMember]
        public int Top { get; set; }
        [DataMember]
        public int Right { get; set; }
        [DataMember]
        public int Bottom { get; set; }
        public POINT TopLeft
            => new POINT(Left, Top);
        public POINT BottomRight
            => new POINT(Bottom, Right);
        public int Width
            => Right - Left;
        public int Height
            => Bottom - Top;
        public POINT Size
            => new POINT(Width, Height);
        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is RECT)
            {
                var rect = (RECT)obj;

                return rect.Bottom == Bottom &&
                       rect.Left == Left &&
                       rect.Right == Right &&
                       rect.Top == Top;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() => Bottom.GetHashCode() ^
       Left.GetHashCode() ^
       Right.GetHashCode() ^
       Top.GetHashCode();

        public static bool operator ==(RECT a, RECT b) => a.Bottom == b.Bottom &&
       a.Left == b.Left &&
       a.Right == b.Right &&
       a.Top == b.Top;

        public static bool operator !=(RECT a, RECT b) => !(a == b);

        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            =>
                $"<{nameof(RECT)}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
            => $"[{TopLeft},{BottomRight} <{Size}>]";
        #endregion
        #endregion

    }

    [Serializable]
    [XmlSerializerAssembly("wUAL.XmlSerializers")]
    [DataContract(Namespace = Namespaces.Default)]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT : IDebuggerDisplay
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [XmlIgnore]
        public static POINT Empty = new POINT(-1, -1);
        [XmlIgnore]
        public static POINT Zero = new POINT(0, 0);
        [XmlIgnore]
        public bool IsEmpty
            => this == Empty;
        [XmlIgnore]
        public bool IsZero
            => this == Zero;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is POINT)
            {
                var point = (POINT)obj;

                return point.X == X && point.Y == Y;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode();

        public static bool operator ==(POINT a, POINT b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(POINT a, POINT b)
            => !(a == b);

        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(POINT)}> {DebuggerDisplaySimple(level)}";

        public string DebuggerDisplaySimple(int level = 1)
            => this.IsEmpty ? "Empty" 
            : this.IsZero ? "Zero" 
            : $"{X}x{Y}";
        #endregion
        #endregion
    }
    public static partial class EnumExtensions
    {
        public static bool IsMinimized(this WindowPlacementState value)
            => value == WindowPlacementState.Minimized;
    }
    public enum WindowPlacementState
    {
        Empty,
        Normal,
        Minimized,
        Maximimized
    }
    [Serializable]
    [XmlSerializerAssembly("wUAL.XmlSerializers")]
    [DataContract(Name="WindowPlacement", Namespace = Namespaces.Default)]
    [KnownType(typeof(POINT)),
        KnownType(typeof(RECT))]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public struct WINDOWPLACEMENT : IDebuggerDisplay
    {
        [DataMember]
        public int length;
        [DataMember]
        public int flags;
        [DataMember]
        public int showCmd;
        [DataMember]
        public POINT minPosition;
        [DataMember]
        public POINT maxPosition;
        [DataMember]
        public RECT normalPosition;
        [XmlIgnore]
        public WindowPlacementState State
        {
            get { return (WindowPlacementState)showCmd; }
            set { showCmd = (int)value; }
        }
        public void UnminimizeState()
        {
            if (State.IsMinimized())
            {
                State = WindowPlacementState.Normal;
            }
        }
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            =>
                $"<{nameof(WINDOWPLACEMENT)}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
        {
            var values = new List<object> {
                "Normal", normalPosition
            };
            if (!minPosition.IsEmpty && !minPosition.IsZero)
            {
                values.AddRange(new object[] { "Min", minPosition });
            }
            if (!maxPosition.IsEmpty && !maxPosition.IsZero)
            {
                values.AddRange(new object[] { "Max", maxPosition });
            }
            values.AddRange(new object[] { "State", State });
            if (flags != 0)
            {
                values.AddRange(new object[] { "Flags", flags });
            }            
            return values.ToStringKeyValue();
        }
        #endregion
        #endregion
    }
    public class WindowPlacementEntry
    {
        public string Key { get; set; } = null;
        public WINDOWPLACEMENT? Placement { get; set; } = null;
        public WindowPlacementEntry() { }
        public WindowPlacementEntry(string key, WINDOWPLACEMENT? placement)
        {
            this.Key = key;
            this.Placement = placement;
        }
    }
}