using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Torrent.Helpers.Utils;
using Torrent.Extensions;

namespace Torrent.Extensions.Reflection
{
    public static class AssemblyExtensions
    {
        public static Type[] GetTypesSafe(this Assembly assembly, string text=null, [CallerMemberName] string source=null)
        {
            Type[] types = null;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var pad = 55;
                types = ex.Types;
                LogUtils.Writers.Types(text ?? assembly.GetName().Name,
                    new string(' ', 5) +
                    ex.LoaderExceptions
                    .Select(e => e.Message).Join(pad), source);
                // Debugger.Break();
            }
            return types;
        }
    }
}
