using System;
using System.Reflection;

namespace MapperExpression.Exception
{
    /// <summary>
    /// Exception for real only property
    /// </summary>
    [Serializable]
    
    public class ReadOnlyPropertyException :System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public ReadOnlyPropertyException(PropertyInfo property)
            : base("The destination property  '" + property.Name + "' is read-only")
        {

        }
    }
}
