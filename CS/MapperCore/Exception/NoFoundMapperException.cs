using System;

namespace MapperExpression.Exception
{
    /// <summary>
    /// Exception when a mapper is not found
    /// </summary>
    [Serializable]
    public class NoFoundMapperException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFoundMapperException"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        public NoFoundMapperException(Type source, Type dest)
        : base("The mapping for the types '" + source.Name + "' et '" + dest.Name + "' are not configured")
        {

        }
    }
}
