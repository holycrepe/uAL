
using System;
using System.Runtime.Serialization;

namespace Torrent.Exceptions
{
    /// <summary>
    /// Exception thrown to indicate that an attempt was made to create a derived Enum Toggle
    /// from an already derived type
    /// </summary>
    public class EnumToggleStateException : Exception
    {
        /// <summary>
        /// Constructs a new instance of EnumToggleStateException with no message.
        /// </summary>
        public EnumToggleStateException()
        {
        }

        /// <summary>
        /// Constructs a new instance of EnumToggleStateException with the given message.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        public EnumToggleStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance of EnumToggleStateException with the given message and inner exception.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        /// <param name="inner">Inner exception.</param>
        public EnumToggleStateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor provided for serialization purposes.
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Context</param>
        protected EnumToggleStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
