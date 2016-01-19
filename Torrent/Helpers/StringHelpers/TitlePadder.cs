namespace Torrent.Helpers.StringHelpers
{
    using System;
    using Enums;
    using Extensions;

    public class TitlePadder : PadStringOptions
    {
        public new const int DEFAULT_WIDTH = 25;
        protected new const PadDirection DEFAULT_DIRECTION = PadDirection.Right;
        protected const string DEFAULT_TITLE_SUFFIX = ":";
        protected override PadDirection DefaultDirection => DEFAULT_DIRECTION;
        public object Text { get; set; } = null;
        public string TitleSuffix { get; set; }
        public string TitleEmptySuffix { get; set; } = " ";
        public string TextPrefix { get; set; } = " ";


        public string GetTitle(object title)
            => title + GetTitleSuffix(title);
        public string GetTitleSuffix(object title)
            => string.IsNullOrEmpty(title?.ToString()) ? TitleEmptySuffix : TitleSuffix;

        public string PadTitle()
            => PadTitle(Subject, Text);
        public string PadTitle(object title, object text=null)
            => text == null ? title?.ToString() : GetTitle(title).Pad(this) + TextPrefix + text;
        //public TitlePadder(object title, object text, int width, bool padDefaultDirection, string titleSuffix = DEFAULT_TITLE_SUFFIX, char paddingChar = DEFAULT_PADDING_CHAR) : 
        //    this(title, text, width, padDefaultDirection ? PadDire : PadDirection.Left, titleSuffix, paddingChar) { }
        public TitlePadder(object title = null, object text = null, int width = DEFAULT_WIDTH, PadDirection direction = PadDirection.Default, string titleSuffix = DEFAULT_TITLE_SUFFIX, char paddingChar = DEFAULT_PADDING_CHAR) {
            Subject = title;
            Text = text;
            Width = width;
            Direction = direction;
            TitleSuffix = titleSuffix;
            PaddingChar = paddingChar;
        }

        public static implicit operator string(TitlePadder padder)
            => padder.PadTitle();
    }
}