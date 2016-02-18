using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    using System;
    using AddGenericConstraint;

    /// <summary>
    /// Provides a set of extension methods for use with enum types. Much of
    /// what's available here is already in System.Enum, but this class
    /// provides a strongly typed API.
    /// </summary>
    public static partial class EnumExtensions
    {        
        public static T[] GetValues<[AddGenericConstraint(typeof(Enum))] T>(this T? value) where T : struct
            => EnumUtils.GetValuesArray<T>(); 

        [Obsolete("Use GetDescription()", true)]
        public static string GetDescriptionOld<[AddGenericConstraint(typeof(Enum))] T>(this T? value) where T : struct
            => EnumUtils.GetDescription(value);        

        /// <summary>
        /// Checks whether the value is a named value for the type.
        /// </summary>
        /// <remarks>
        /// For flags enums, it is possible for a value to be a valid
        /// combination of other values without being a named value
        /// in itself. To test for this possibility, use IsValidCombination.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to test</param>
        /// <returns>True if this value has a name, False otherwise.</returns>
        public static bool IsNamedValue<T>(this T value) where T : struct
            // TODO: Speed this up for big enums
            => EnumUtils.GetValues<T>().Contains(value);

        /// <summary>
        /// Returns the description for the given value, 
        /// as specified by DescriptionAttribute, or null
        /// if no description is present.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="item">Value to fetch description for</param>
        /// <returns>The description of the value, or null if no description
        /// has been specified (but the value is a named value).</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="item"/>
        /// is not a named member of the enum</exception>
        public static string GetDescription<[AddGenericConstraint(typeof(Enum))] T>(this T item) where T : struct
        {
            string description;
            if (EnumInternals<T>.ValueToDescriptionMap.TryGetValue(item, out description))
            {
                return description;
            }
            throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}
