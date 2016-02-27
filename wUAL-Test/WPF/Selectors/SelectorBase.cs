using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using Torrent.Extensions;
using uAL.Extensions;
using System;

namespace wUAL.WPF.Selectors
{
    public static partial class Selectors {
        public class SelectorBase<T>
            where T : class
        {
            public SelectorBase()
            {

            }
            public SelectorBase(Func<T, object[]> GetSelectorsFunc)
            {
                this.GetSelectors = GetSelectorsFunc;
            }

            public TSelector GetSelections<TSelector>(T item, Dictionary<string, TSelector> selections)
            {
                if (item == default(T))
                    return default(TSelector);
                var selectors = this.GetSelectors(item);
                for (var i = 0; i < selectors.Length; i += 2)
                {
                    var flag = true;
                    if (i + 1 < selectors.Length)
                        flag = (bool)selectors[i + 1];
                    var selector = (string) selectors[i];
                    TSelector result;
                    if (selections.TryGetSelection(selector, out result, flag))
                        return result;
                }
                return default(TSelector);
            }
            protected virtual Func<T,object[]> GetSelectors { get; }
        }
    }
}
