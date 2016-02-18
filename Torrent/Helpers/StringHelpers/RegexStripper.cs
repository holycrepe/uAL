namespace Torrent.Helpers.StringHelpers
{
    using System.Text.RegularExpressions;
    using Extensions;
    using static Utils.RegexUtils;

    public class RegexStripper
    {
        public readonly string Original;
        public readonly string Stripped;
        public readonly string Final;
        public readonly string StripWords;
        public readonly string WordsPattern;
        public readonly string Pattern;
        public readonly bool Extended;

        public RegexStripper(string subject, bool extended = false)
        {
            Extended = extended;
            StripWords = CreateGroup(GetLabelsWordsToStrip(Extended));
            WordsPattern = CreateBoundary(StripWords);
            Pattern = CreateGroup(WordsPattern, TokenGroup, BoundaryTokensStart, BoundaryTokensEnd);
            Original = subject;
            Stripped = ReplaceAll(Original, Pattern, " ").UnescapeHTML().TrimAll();
            Final = Regex.Replace(Stripped, (Extended ? @"\P{L}+" : " +"), " ", RegexOptions.IgnoreCase).TrimAll();
        }
    }
}
