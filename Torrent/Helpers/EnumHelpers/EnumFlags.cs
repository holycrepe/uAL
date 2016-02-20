using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AddGenericConstraint;
using Torrent.Exceptions;
using Torrent.Extensions;

namespace Torrent.Helpers.Utils
{
    /// <summary>
    /// Provides a set of static methods for use with "flags" enums,
    /// i.e. those decorated with <see cref="FlagsAttribute"/>.
    /// Methods in this class throw <see cref="ArgumentTypeException" />.
    /// </summary>
    public static partial class EnumUtils
    {
        /// <summary>
        /// Helper method used by almost all methods to make sure
        /// the type argument is really a flags enum.
        /// </summary>
        static void ThrowIfNotFlags<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
        {
            if (!EnumInternals<T>.IsFlags)
            {
                throw new ArgumentTypeException("Can't call this method for a non-flags enum");
            }
        }

        /// <summary>
        /// Helper method that evaluates and returns the expression if and only if
        /// the type argument is really a flags enum.
        /// </summary>
        internal static T EnsureIsFlags<[AddGenericConstraint(typeof(Enum))] T>(Func<T> expression) where T : struct
        {
            ThrowIfNotFlags<T>();
            return expression();
        }

        /// <summary>
        /// Helper method that evaluates and returns the expression if and only if
        /// the type argument is really a flags enum.
        /// </summary>
        internal static TReturn EnsureIsFlags<[AddGenericConstraint(typeof(Enum))] T, TReturn>(T value, Func<TReturn> expression) where T : struct
        {
            ThrowIfNotFlags<T>();
            return expression();
        }

        /// <summary>
        /// Returns whether or not the specified enum is a "flags" enum,
        /// i.e. whether it has FlagsAttribute applied to it.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>True if the enum type is decorated with
        /// FlagsAttribute; False otherwise.</returns>
        public static bool IsFlags<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => EnumInternals<T>.IsFlags;


        /// <summary>
        /// Returns whether or not the specified enum is a "flags" enum,
        /// i.e. whether it has FlagsAttribute applied to it.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>True if the enum type is decorated with
        /// FlagsAttribute; False otherwise.</returns>
        public static bool IsFlags(Type enumType)
            => enumType.IsEnum && enumType.GetAttribute<FlagsAttribute>() != null;
        /// <summary>
        /// Returns all the bits used in any flag values
        /// </summary>
        /// internal static
        /// <returns>A flag value with all the bits set that are ever set in any defined value</returns>
        /// <exception cref="ArgumentTypeException"><typeparamref name="T"/> is not a flags enum.</exception>
        public static T GetUsedBits<[AddGenericConstraint(typeof(Enum))] T>() where T : struct
            => EnsureIsFlags(() => EnumInternals<T>.UsedBits);

        
    }
}
