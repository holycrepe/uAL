namespace Torrent.Helpers.Utils
{
    using System;
    using System.Collections.Generic;

    public static partial class RegexUtils
    {
    	const string WORD_BOUNDARY = @"[^a-z0-9]+";
    	const string WORD_BOUNDARY_START = @"(?:^|" + WORD_BOUNDARY + ")";
        const string WORD_BOUNDARY_END = @"(?:$|" + WORD_BOUNDARY + ")";
        private static readonly string[] Tokens = { @"\( *\)", @"\[ *\]", @"\{ *\}", @"< *>", @"^ +\-+ +", @" +\-+ +$" };
        private static readonly string[] BoundaryTokens = { @" +\-+ +" };

        public static readonly string TokenGroup = CreateGroup(Tokens);
        public static readonly string BoundaryTokensStart = CreateGroup("^" + CreateGroup(BoundaryTokens));
        public static readonly string BoundaryTokensEnd = CreateGroup(CreateGroup(BoundaryTokens) + "$");
        public static Func<bool, IEnumerable<string>> GetLabelsWordsToStrip = (extended) => "".Split();
    }
}
