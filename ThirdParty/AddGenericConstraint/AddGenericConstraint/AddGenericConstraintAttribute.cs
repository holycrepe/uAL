using System;
using PostSharp.Extensibility;

namespace AddGenericConstraint
{
    [AttributeUsage( AttributeTargets.GenericParameter )]
    [RequirePostSharp( "AddGenericConstraint", "AddGenericConstraint" )]
    public sealed class AddGenericConstraintAttribute : Attribute
    {
        public AddGenericConstraintAttribute( Type type )
        {
        }
    }
}