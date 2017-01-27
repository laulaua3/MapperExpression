using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace MapperExpression.Exceptions
{
    /// <summary>
    /// Exception when a mapper is not Initialized
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public  class MapperNotInitializedException : MapperBaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperNotInitializedException"/> class.
        /// </summary>
        /// <param name="typeSource">The type source.</param>
        /// <param name="typeDest">The type dest.</param>
        public MapperNotInitializedException(Type typeSource, Type typeDest)
          : base(ValideParameter("Mapper for the source type '" + typeSource.FullName + "' and the type of destination '" + typeDest.FullName + "' is not initialized (called Mapper.Initialise())",
              typeSource!=null,
              typeDest !=null))
      {

      }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperNotInitializedException"/> class.
        /// </summary>
        public MapperNotInitializedException()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperNotInitializedException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public MapperNotInitializedException(string exceptionMessage)
            : base(exceptionMessage)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperNotInitializedException"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="context">The context.</param>
        protected MapperNotInitializedException(SerializationInfo serializer, StreamingContext context)
            :base(serializer, context)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperBaseException"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MapperNotInitializedException(string exceptionMessage, Exception innerException)
            : base(exceptionMessage,innerException)
        {

        }
    }
}
