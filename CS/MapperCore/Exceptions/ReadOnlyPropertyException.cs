using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception for real only property
    /// </summary>
    [Serializable]
    
    public class ReadOnlyPropertyException : MapperExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public ReadOnlyPropertyException(PropertyInfo property)
            : base(ValidateParameter(property))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyException"/> class.
        /// </summary>
        public ReadOnlyPropertyException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected ReadOnlyPropertyException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExceptionBase"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ReadOnlyPropertyException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public ReadOnlyPropertyException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        private static string ValidateParameter(PropertyInfo property)
        {
            Contract.Requires(property != null);

            return "The destination property  '" + property.Name + "' is read-only";
        }
    }
}
