using System;

namespace MapperExpression.Exception
{
    /// <summary>
    /// Exception when the property is not found
    /// </summary>
    [Serializable]
    public class PropertyNoExistException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNoExistException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="typeObject">The type object.</param>
        public PropertyNoExistException(string propertyName,Type typeObject)
            : base("The property '" + propertyName + "' does not exist for the type'" + typeObject.ToString() + "'")
        {
        
        }

    }
}
