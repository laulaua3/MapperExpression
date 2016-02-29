using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperExpression.Exception
{

    /// <summary>
    /// Exception when no action can to be execute
    /// </summary>
    /// <seealso cref="MapperExpression.Exception.MapperExceptionBase" />
    [Serializable]
    public class NoActionAfterMappingException : MapperExceptionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoActionAfterMappingException"/> when no action can to be execute.
        /// </summary>
        public NoActionAfterMappingException()
            : base("The action can not be executed because it is not defined")
        { }

    }
}
