using System;

namespace MapperExpression.Exception
{
    /// <summary>
    /// Exception when a mapper is not Initialized
    /// </summary>
    [Serializable]
    public  class MapperNotInitializedException :System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperNotInitializedException"/> class.
        /// </summary>
        /// <param name="typeSource">The type source.</param>
        /// <param name="typeDest">The type dest.</param>
        public MapperNotInitializedException(Type typeSource, Type typeDest)
          : base("Mapper for the source type '" + typeSource.FullName + "' and the type of destination '" + typeDest.FullName + "' is not initialized (called Mapper.Initialise())")
      {

      }
    }
}
