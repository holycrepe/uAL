namespace Torrent.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Helpers.StringHelpers;
    using Infrastructure;

    public static class DebuggerDisplayExtensions
    {
        public static string GetDebuggerDisplay<T>(this List<T> list, 
    	                                           int level=1,
    	                                           string sep = "\n", string delim = "",
    	                                           int indent = 4, char indentChar = ' ',
                                                   bool combineDelimAndSep = true, NumberPadder padder = null) 
    		where T : IDebuggerDisplay
        {
    		padder = padder ?? new NumberPadder();
            var indentStr = new string(indentChar, level * indent);
            return $"{list.GetType().Name}: " +
                (list.Count == 0 ? "Empty" :
                $"<{list.Count}>{sep}" +
                string.Join(combineDelimAndSep ? delim + sep : delim,
                            list.Select((s, i) => indentStr + padder.PadIndex(i) + s.DebuggerDisplaySimple(level + 1))
                ));
        }
    	
    	public static string GetDebuggerDisplay<TKey, TValue>(this Dictionary<TKey, TValue> dict, int level=1, 
    	                                                      string sep = "\n", string delim = "",
    	                                                      int indent = 4, char indentChar = ' ',
    	                                                      bool combineDelimAndSep = true, NumberPadder padder = null, bool includeTypeName = true)
    		where TValue : IDebuggerDisplay
        {
            padder = padder ?? new NumberPadder();
            var titlePadder = new TitlePadder { 
            	Width = dict.Keys.Max(key => key.ToString().Length) + 3
            };
            var indentStr = new string(indentChar, level * indent);
            
        	return (includeTypeName ? $"{dict.GetType().Name}: " : "") +
                (dict.Count == 0 ? "Empty" :
                $"<{dict.Count}>{sep}" +
                string.Join(combineDelimAndSep ? delim + sep : delim,
                dict.Select((s, i) => 
                                        indentStr + padder.PadIndex(i) + titlePadder.PadTitle($"`{s.Key}`",  s.Value.DebuggerDisplaySimple(level + 1))
                           )
                ));
        }

        public static string GetDebuggerDisplaySimple<TKey, TValue>(this Dictionary<TKey, TValue> dict, int level=1, string delim = ", ", string sep = "")
            where TValue : IDebuggerDisplay
            => dict.GetDebuggerDisplay(level, sep, delim);

        public static string GetDebuggerDisplaySimple<T>(this List<T> list, int level=1, string delim = ", ", string sep = "")
            where T : IDebuggerDisplay
            => list.GetDebuggerDisplay(level, sep, delim);
    }
}