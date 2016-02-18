using System.Collections.Generic;
using PostSharp.CodeModel;
using PostSharp.Extensibility;
using PostSharp.Extensibility.Tasks;

namespace AddGenericConstraint.Impl
{
    public class AddGenericConstraintTask : Task
    {
        public override bool Execute()
        {
            // Get the type AddGenericConstraintAttribute.
            ITypeSignature addGenericConstraintAttributeType = this.Project.Module.FindType(
                typeof(AddGenericConstraintAttribute),
                BindingOptions.OnlyDefinition | BindingOptions.DontThrowException );

            if ( addGenericConstraintAttributeType == null )
            {
                // The type is not referenced in the current module. There cannot be a custom attribute
                // of this type, so we are done.
                return true;
            }

            // Enumerate custom attributes of type AddGenericConstraintAttribute.
            AnnotationRepositoryTask annotationRepository = AnnotationRepositoryTask.GetTask( this.Project );
            IEnumerator<IAnnotationInstance> customAttributesEnumerator =
                annotationRepository.GetAnnotationsOfType( addGenericConstraintAttributeType.GetTypeDefinition(), false );
            while ( customAttributesEnumerator.MoveNext() )
            {
                // Get the target of the custom attribute.
                GenericParameterDeclaration target = (GenericParameterDeclaration) customAttributesEnumerator.Current.TargetElement;

                // Get the value of the parameter of the custom attribute constructor.
                ITypeSignature constraintType = (ITypeSignature) customAttributesEnumerator.Current.Value.ConstructorArguments[0].Value.Value;

                // Add a generic constraint.
                target.Constraints.Add( constraintType );

                // Remove the custom attribute.
                ((CustomAttributeDeclaration) customAttributesEnumerator.Current).Remove();
            }

            return base.Execute();
        }
    }
}