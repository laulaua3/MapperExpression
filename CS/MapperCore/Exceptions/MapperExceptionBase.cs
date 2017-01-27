using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;


namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Mapper exception. 
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class MapperBaseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public MapperBaseException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        public MapperBaseException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected MapperBaseException(SerializationInfo serializer, StreamingContext context)
            : base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MapperBaseException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage, innerException)
        {

        }

        /// <summary>
        /// Valides the parameter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="conditions">The conditions.</param>
        /// <returns>The message if no exception</returns>
        protected static string ValideParameter(string message, params bool[] conditions)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                Contract.Requires(conditions[i]);
            }
            return message;
        }
    }
}
