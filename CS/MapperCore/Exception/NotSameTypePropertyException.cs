
using System;
namespace MapperExpression.Exception
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
            : base("The source and destination properties are not the same type or no mapper is found(source type " + typeSource.Name + " destination and type " + typeDest.Name + ")")
        {

        }
    }
}
