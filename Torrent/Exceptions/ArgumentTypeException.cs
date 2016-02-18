
using System;
using System.Runtime.Serialization;

namespace Torrent.Exceptions
{
    /// <summary>
    /// Exception thrown to indicate that an inappropriate type argument was used for
    /// a type parameter to a generic type or method or for an argument of type System.Type
    /// </summary>
    public class ArgumentTypeException : ArgumentException
    {
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class.
        public ArgumentTypeException() { }
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class with a specified
        //     error message.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        public ArgumentTypeException(string message) : base(message) { }
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class with a specified
        //     error message and a reference to the inner exception that is the cause of this
        //     exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not a null reference, the current exception is raised in a catch
        //     block that handles the inner exception.
        public ArgumentTypeException(string message, Exception innerException)
            : base(message, innerException) { }
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class with a specified
        //     error message and the name of the parameter that causes this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   paramName:
        //     The name of the parameter that caused the current exception.
        public ArgumentTypeException(string message, string paramName)
            : base(message, paramName) { }
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class with a specified
        //     error message, the parameter name, and a reference to the inner exception that
        //     is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   paramName:
        //     The name of the parameter that caused the current exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not a null reference, the current exception is raised in a catch
        //     block that handles the inner exception.
        public ArgumentTypeException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException) { }
        //
        // Summary:
        //     Initializes a new instance of the ArgumentTypeException class with serialized
        //     data.
        //
        // Parameters:
        //   info:
        //     The object that holds the serialized object data.
        //
        //   context:
        //     The contextual information about the source or destination.
        protected ArgumentTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
