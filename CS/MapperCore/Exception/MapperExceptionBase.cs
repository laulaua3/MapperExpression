using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperExpression.Exception
{
    /// <summary>
    /// mapper exception 
    /// </summary>
    [Serializable]
    public class MapperExceptionBase : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapperExceptionBase"/> class.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        public MapperExceptionBase(string exceptionMessage)
            :base(exceptionMessage)
        {

        }
    }
}
