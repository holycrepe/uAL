using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent.Infrastructure.Enums.Toggles
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Reflection;
    using AddGenericConstraint;
    using Extensions;
    using global::Torrent.Exceptions;
    using Puchalapalli.Dynamic;
    public static class EnumToggleExtensions
    {
        public static TSettings GetActiveToggles<TSettings, [AddGenericConstraint(typeof(Enum))] TEnum>(this IEnumToggles<TEnum, TEnum> toggles, TEnum flag)
            where TEnum : struct
            where TSettings : IEnumToggles<TEnum, bool>, new()
            => (TSettings)toggles.GetActiveToggles(flag, typeof(TSettings));

        internal static object GetActiveToggles<[AddGenericConstraint(typeof(Enum))] TEnum>(this IEnumToggles<TEnum, TEnum> toggles, TEnum flag, Type settingsType = null) 
            where TEnum : struct
        {
            //var toggle = Activator.CreateInstance(toggles.ToggleType, new object[] { toggles, flag });
            //var toggle = Activator.CreateInstance(toggles.ToggleType);
            // ReSharper disable once AssignNullToNotNullAttribute
            var toggle = Activator.CreateInstance(settingsType ?? toggles.ToggleType);
            (toggle as IEnumToggles<TEnum, bool>)?.SetToggles(toggles, flag);
            return toggle;
        }

    }
}
