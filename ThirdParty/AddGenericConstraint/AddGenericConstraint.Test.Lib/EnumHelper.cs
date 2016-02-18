using System;

namespace AddGenericConstraint.Test
{
    public static class EnumHelper
    {
        public static T[] GetValues<[AddGenericConstraint( typeof(Enum) )] T>() where T : struct
        {
            return (T[]) Enum.GetValues( typeof(T) );
        }
    }
}