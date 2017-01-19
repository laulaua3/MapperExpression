using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MapperExpression.Exceptions
{

    /// <summary>
    /// Exception when no action can to be execute
    /// </summary>
    /// <seealso cref="MapperExpression.Exceptions.MapperBaseException" />
    [Serializable]
    public class NoActionAfterMappingException : MapperBaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoActionAfterMappingException"/> when no action can to be execute.
        /// </summary>
        public NoActionAfterMappingException()
            : base("The action can not be executed because it is not defined")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NoActionAfterMappingException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected NoActionAfterMappingException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NoActionAfterMappingException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public NoActionAfterMappingException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoActionAfterMappingException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }

    }
}
