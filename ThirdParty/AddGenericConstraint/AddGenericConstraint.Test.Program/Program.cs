using System;

namespace AddGenericConstraint.Test.Program
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            foreach ( TypeCode typeCode in EnumHelper.GetValues<TypeCode>() )
            {
                Console.WriteLine( typeCode );
            }

            // This line will give an error.
            EnumHelper.GetValues<DateTime>();
        }
    }
}