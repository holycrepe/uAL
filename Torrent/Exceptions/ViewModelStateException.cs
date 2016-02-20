
using System;
using System.Runtime.Serialization;

namespace Torrent.Exceptions
{
    /// <summary>
    /// Exception thrown to indicate that an attempt access a property of the View Model was invalid
    /// </summary>
    public class ViewModelStateException : Exception
    {
        /// <summary>
        /// Constructs a new instance of ViewModelStateException with no message.
        /// </summary>
        public ViewModelStateException()
        {
        }

        /// <summary>
        /// Constructs a new instance of ViewModelStateException with the given message.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        public ViewModelStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance of ViewModelStateException with the given message and inner exception.
        /// </summary>
        /// <param name="message">Message for the exception.</param>
        /// <param name="inner">Inner exception.</param>
        public ViewModelStateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor provided for serialization purposes.
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Context</param>
        protected ViewModelStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
