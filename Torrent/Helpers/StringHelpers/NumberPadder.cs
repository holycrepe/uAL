namespace Torrent.Helpers.StringHelpers
{
    using Extensions;

    public class NumberPadder : PadStringOptions
    {
        public int Index { get; set; }
        public int Number => GetNumber(Index);
        public string Separator { get; set; } = ". ";
        public bool IsIndex { get; set; }

        public NumberPadder(int index = -1, bool useValue = true, int width = 0, bool isIndex = true)
        {
            Index = index;
            Width = width == 0 ? 3 : width;
            UseValue = useValue;
            IsIndex = isIndex;
        }
        int GetNumber(int index)
            => index + (IsIndex ? 1 : 0);

        public string PadIndex()
            => PadIndex(Index);
        public string PadIndex(int index) 
            => UseValue ? GetNumber(index).ToString().Pad(this) + Separator : "";

        public static implicit operator string(NumberPadder helper)
            => helper.PadIndex();
    }
}