
using System;
using System.Runtime.Serialization;

namespace Torrent.Exceptions
{
    /// <summary>
    /// Exception thrown to indicate that an attempt to set a property of a dynamic object failed
    /// because the property or object is read-only
    /// </summary>
    public class DynamicReadOnlyException : Exception
    {
        /// <summary>
        /// Constructs a new instance of DynamicReadOnlyException with no message.
        /// </summary>
        public DynamicReadOnlyException()
        {
        }

        /// <summary>
        /// Constructs a new instance of DynamicReadOnlyException with the given message.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        public DynamicReadOnlyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance of DynamicReadOnlyException with the given message and inner exception.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        /// <param name="inner">Inner exception.</param>
        public DynamicReadOnlyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor provided for serialization purposes.
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Context</param>
        protected DynamicReadOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
