using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Torrent.Infrastructure.Exceptions;

namespace Torrent.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<ExceptionSummary> GetSummaries(this IEnumerable<Exception> exceptions,
                                                                 int padding = -1)
        => exceptions.Select(ex => ex.GetSummary(padding));
        public static ExceptionSummary GetSummary(this Exception ex, int padding=-1)
            => new ExceptionSummary(ex, padding: padding);
        public static string GetSummaryText(this Exception ex, int padding=-1)
            => ex.GetSummary(padding).ToString();
    }
}