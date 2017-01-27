using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception when the property is not found
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class PropertyNoExistException : MapperBaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNoExistException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeObject">The type object.</param>
        public PropertyNoExistException(string propertyName,Type typeObject)
            : base(ValideParameter("The property '" + propertyName + "' does not exist for the type'" + typeObject.ToString() + "'", !String.IsNullOrEmpty(propertyName),typeObject != null))
        {
        
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNoExistException"/> class.
        /// </summary>
        public PropertyNoExistException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNoExistException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public PropertyNoExistException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNoExistException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected PropertyNoExistException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PropertyNoExistException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }
    }
}
