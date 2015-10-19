using System;

namespace MapperExpression.Exception
{
    /// <summary>
    /// Exception when a mapper is already exist
    /// </summary>
    [Serializable]
    public class MapperExistException:System.Exception
   
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExistException"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dest">The dest.</param>
        public MapperExistException(Type source,Type dest)
            : base("A mapper already exists for the type of source '" + source.FullName + "' and the type of destination '" + dest.FullName + "'")
        {

        }
    }
}
