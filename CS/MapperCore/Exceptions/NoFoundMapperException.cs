using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception when a mapper is not found
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class NoFoundMapperException : MapperBaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFoundMapperException"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        public NoFoundMapperException(Type source, Type dest)
        : base(ValideParameter($"The mapping for the types '{source.Name}' and '{dest.Name }' are not configured, use 'Mapper.Create<{source.Name},{dest.Name }>();' to register it.", source != null, dest != null))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFoundMapperException"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NoFoundMapperException(string name)
        : base(ValideParameter("The mapping with the name " + name + " was not found.", !string.IsNullOrEmpty(name)))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFoundMapperException"/> class.
        /// </summary>
        public NoFoundMapperException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFoundMapperException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected NoFoundMapperException(SerializationInfo serializer, StreamingContext context)
            : base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoFoundMapperException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage, innerException)
        {

        }
    }
}
