using System;
using System.Diagnostics;
using Torrent.Infrastructure.Exceptions;

namespace Torrent.Extensions
{
    public static class ExceptionExtensions
    {
        public static ExceptionSummary GetSummary(this Exception ex)
            => new ExceptionSummary(ex);
        public static string GetSummaryText(this Exception ex)
            => ex.GetSummary().ToString();
    }
}