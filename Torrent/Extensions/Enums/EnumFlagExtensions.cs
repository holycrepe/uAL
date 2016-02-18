using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    using System;
    using AddGenericConstraint;
    using static Helpers.Utils.EnumUtils;

    /// <summary>
    /// Provides a set of extension methods for use with "flags" enums,
    /// i.e. those decorated with <see cref="FlagsAttribute"/>.
    /// Other than <see cref="IsValidCombination{T}"/>, methods in this
    /// class throw <see cref="TypeException" />.
    /// </summary>
    public static class EnumFlagExtensions
    {
        /// <summary>
        /// Determines whether the given value only uses bits covered
        /// by named values.
        /// </summary>
        /// internal static
        /// <param name="values">Combination to test</param>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static bool IsValidCombination<[AddGenericConstraint(typeof(Enum))] T>(this T values) where T : struct
            => EnsureIsFlags(values, () => values.And(EnumInternals<T>.UnusedBits).IsEmpty());

        /// <summary>
        /// Determines whether the two specified values have any flags in common.
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <param name="desiredFlags">Flags we wish to find</param>
        /// <returns>Whether the two specified values have any flags in common.</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static bool HasAny<[AddGenericConstraint(typeof(Enum))] T>(this T value, T desiredFlags) where T : struct
            => EnsureIsFlags(value, () => value.And(desiredFlags).IsNotEmpty());

        /// <summary>
        /// Determines whether all of the flags in <paramref name="desiredFlags"/>
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <param name="desiredFlags">Flags we wish to find</param>
        /// <returns>Whether all the flags in <paramref name="desiredFlags"/> are in <paramref name="value"/>.</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static bool Has<[AddGenericConstraint(typeof(Enum))] T>(this T value, T desiredFlags) where T : struct
            => EnsureIsFlags(value, () => EnumInternals<T>.Equality(value.And(desiredFlags), desiredFlags));

        /// <summary>
        /// Returns the bitwise "and" of two values.
        /// </summary>
        /// internal static
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <returns>The bitwise "and" of the two values</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static T And<[AddGenericConstraint(typeof(Enum))] T>(this T first, T second) where T : struct
            => EnsureIsFlags(first, () => EnumInternals<T>.And(first, second));

        /// <summary>
        /// Returns the bitwise "and" of two values.
        /// </summary>
        /// internal static
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <returns>The bitwise "and" of the two values</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static void AddTo<[AddGenericConstraint(typeof(Enum))] T>(this T newFlag, ref T baseFlag) where T : struct
        {
            var value = baseFlag;
            baseFlag = EnsureIsFlags(baseFlag, () => EnumInternals<T>.And(value, newFlag));
        }

        /// <summary>
        /// Returns the bitwise "or" of two values.
        /// </summary>
        /// internal static
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <returns>The bitwise "or" of the two values</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static T Or<[AddGenericConstraint(typeof(Enum))] T>(this T first, T second) where T : struct
            => EnsureIsFlags(first, () => EnumInternals<T>.Or(first, second));

        /// <summary>
        /// Returns the inverse of a value, with no consideration for which bits are used
        /// by values within the enum (i.e. a simple bitwise negation).
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to invert</param>
        /// <returns>The bitwise negation of the value</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static T AllBitsInverse<[AddGenericConstraint(typeof(Enum))] T>(this T value) where T : struct
            => EnsureIsFlags(() => EnumInternals<T>.Not(value));

        /// <summary>
        /// Returns the inverse of a value, but limited to those bits which are used by
        /// values within the enum.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to invert</param>
        /// <returns>The restricted inverse of the value</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static T UsedBitsInverse<[AddGenericConstraint(typeof(Enum))] T>(this T value) where T : struct
            => EnsureIsFlags(() => value.AllBitsInverse().And(EnumInternals<T>.UsedBits));

        /// <summary>
        /// Returns whether this value is an empty set of fields, i.e. the zero value.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to test</param>
        /// <returns>True if the value is empty (zero); False otherwise.</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static bool IsEmpty<[AddGenericConstraint(typeof(Enum))] T>(this T value) where T : struct
            => EnsureIsFlags(() => EnumInternals<T>.IsEmpty(value));

        /// <summary>
        /// Returns whether this value has any fields set, i.e. is not zero.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Value to test</param>
        /// <returns>True if the value is non-empty (not zero); False otherwise.</returns>
        /// <exception cref="TypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static bool IsNotEmpty<[AddGenericConstraint(typeof(Enum))] T>(this T value) where T : struct
            => EnsureIsFlags(value, () => !value.IsEmpty());
    }
}
