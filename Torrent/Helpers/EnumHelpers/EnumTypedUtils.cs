using AddGenericConstraint;
using System;
using System.Collections.Generic;

namespace Torrent.Helpers.Utils
{
    /// <summary>
    /// Provides a set of static methods for use with enum types. Much of
    /// what's available here is already in System.Enum, but this class
    /// provides a strongly typed API.
    /// </summary>
    public static partial class EnumUtils
    {
        /// <summary>
        /// Returns an array of values in the enum.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>An array of values in the enum</returns>
        public static T[] GetValuesArray<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => (T[])Enum.GetValues(typeof(T));

        /// <summary>
        /// Returns the values for the given enum as an immutable list.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        public static IList<T> GetValues<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => EnumInternals<T>.Values;

        /// <summary>
        /// Returns an array of names in the enum.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>An array of names in the enum</returns>
        public static string[] GetNamesArray<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => Enum.GetNames(typeof(T));

        /// <summary>
        /// Returns the names for the given enum as an immutable list.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>An array of names in the enum</returns>
        public static IList<string> GetNames<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => EnumInternals<T>.Names;

        /// <summary>
        /// Attempts to find a value with the given description.
        /// </summary>
        /// <remarks>
        /// More than one value may have the same description. In this unlikely
        /// situation, the first value with the specified description is returned.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="description">Description to find</param>
        /// <param name="value">Enum value corresponding to given description (on return)</param>
        /// <returns>True if a value with the given description was found,
        /// false otherwise.</returns>
        public static bool TryParseDescription<[AddGenericConstraint(typeof(Enum))] T>(string description, out T value)
            where T : struct
            => EnumInternals<T>.DescriptionToValueMap.TryGetValue(description, out value);

        /// <summary>
        /// Parses the name of an enum value.
        /// </summary>
        /// <remarks>
        /// This method only considers named values: it does not parse comma-separated
        /// combinations of flags enums.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>The parsed value</returns>
        /// <exception cref="ArgumentException">The name could not be parsed.</exception>
        public static T ParseName<[AddGenericConstraint(typeof(Enum))] T>(string name) where T : struct
        {
            T value;
            if (!TryParseName(name, out value))
            {
                throw new ArgumentException("Unknown name", "name");
            }
            return value;
        }

        /// <summary>
        /// Attempts to find a value for the specified name.
        /// Only names are considered - not numeric values.
        /// </summary>
        /// <remarks>
        /// If the name is not parsed, <paramref name="value"/> will
        /// be set to the zero value of the enum. This method only
        /// considers named values: it does not parse comma-separated
        /// combinations of flags enums.
        /// </remarks>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="name">Name to parse</param>
        /// <param name="value">Enum value corresponding to given name (on return)</param>
        /// <returns>Whether the parse attempt was successful or not</returns>
        public static bool TryParseName<[AddGenericConstraint(typeof(Enum))] T>(string name, out T value) where T : struct
        {
            // TODO: Speed this up for big enums
            int index = EnumInternals<T>.Names.IndexOf(name);
            if (index == -1)
            {
                value = default(T);
                return false;
            }
            value = EnumInternals<T>.Values[index];
            return true;
        }

        /// <summary>
        /// Returns the underlying type for the enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>The underlying type (Byte, Int32 etc) for the enum</returns>
        public static Type GetUnderlyingType<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => EnumInternals<T>.UnderlyingType;
    }
}
