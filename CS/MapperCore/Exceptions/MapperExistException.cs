using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception when a mapper is already exist
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class MapperExistException: MapperBaseException

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExistException"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        public MapperExistException(Type source,Type dest)
            : base(ValideParameter("A mapper already exists for the type of source '" + source.FullName + "' and the type of destination '" + dest.FullName + "'",
                source!=null,
                dest!= null))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExistException"/> class.
        /// </summary>
        public MapperExistException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExistException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public MapperExistException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExistException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected MapperExistException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MapperExistException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }
    }
}
