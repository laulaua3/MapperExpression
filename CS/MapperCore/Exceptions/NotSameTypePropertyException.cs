
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception when the properties aren't the same type or no mapper found
    /// </summary>
    [Serializable]
    public class NotSameTypePropertyException : MapperExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSameTypePropertyException"/> class.
        /// </summary>
        /// <param name="typeSource">The type source.</param>
        /// <param name="typeDest">The type dest.</param>
        public NotSameTypePropertyException(Type typeSource, Type typeDest)
            : base(ValidateParameter(typeSource, typeDest))
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSameTypePropertyException"/> class.
        /// </summary>
        public NotSameTypePropertyException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSameTypePropertyException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected NotSameTypePropertyException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExceptionBase"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotSameTypePropertyException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NotSameTypePropertyException"/> class.
        /// </summary>
        public NotSameTypePropertyException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }

        private static string ValidateParameter(Type typeSource, Type typeDest)
        {
            Contract.Requires(typeSource != null);
            Contract.Requires(typeDest != null);
            return "The source and destination properties are not the same type or no mapper is found(source type " + typeSource.Name + " destination and type " + typeDest.Name + ")";
        }
    }
}
